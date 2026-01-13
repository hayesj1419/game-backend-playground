using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

// --------------------
// App setup
// --------------------

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

// --------------------
// Global state
// --------------------

var gameState = new GameState();
var connections = new ConnectionManager();

// Start game loop
var gameLoop = new GameLoop(gameState, connections, tickRate: 2);
gameLoop.Start();

// --------------------
// WebSocket endpoint
// --------------------

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var socket = await context.WebSockets.AcceptWebSocketAsync();

    var playerId = Guid.NewGuid();
    var player = new Player { Id = playerId };

    gameState.Players[playerId] = player;
    connections.Add(playerId, socket);

    Console.WriteLine($"Player connected: {playerId}");

    await HandleClient(socket, playerId, gameState);

    connections.Remove(playerId);
    gameState.Players.TryRemove(playerId, out _);

    Console.WriteLine($"Player disconnected: {playerId}");
});

app.MapGet("/", () => "Game server running");

app.Run();

// --------------------
// WebSocket handler
// --------------------

static async Task HandleClient(
    WebSocket socket,
    Guid playerId,
    GameState gameState)
{
    var buffer = new byte[1024];

    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer),
            CancellationToken.None
        );

        if (result.MessageType == WebSocketMessageType.Close)
            break;

        var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

        PlayerInput? input;
        try
        {
            input = JsonSerializer.Deserialize<PlayerInput>(
                json,
                Serialization.JsonOptions
            );
        }
        catch
        {
            continue;
        }

        if (input == null)
            continue;

        if (gameState.Players.TryGetValue(playerId, out var player))
        {
            player.InputX = input.X;
            player.InputY = input.Y;
        }
    }

    await socket.CloseAsync(
        WebSocketCloseStatus.NormalClosure,
        "Closing",
        CancellationToken.None
    );
}

// --------------------
// Game loop
// --------------------

public class GameLoop
{
    private readonly GameState _state;
    private readonly ConnectionManager _connections;
    private readonly TimeSpan _tickInterval;

    public GameLoop(GameState state, ConnectionManager connections, int tickRate)
    {
        _state = state;
        _connections = connections;
        _tickInterval = TimeSpan.FromMilliseconds(1000.0 / tickRate);
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                var start = stopwatch.Elapsed;

                Tick();

                var elapsed = stopwatch.Elapsed - start;
                var delay = _tickInterval - elapsed;

                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay);
            }
        });
    }

    private void Tick()
    {
        const float speedPerTick = 0.1f;
        var snapshots = new List<PlayerSnapshot>();

        foreach (var player in _state.Players.Values)
        {
            player.X += player.InputX * speedPerTick;
            player.Y += player.InputY * speedPerTick;

            snapshots.Add(new PlayerSnapshot(player.Id, player.X, player.Y));
        }

        BroadcastSnapshots(snapshots);
    }

    private void BroadcastSnapshots(List<PlayerSnapshot> snapshots)
    {
        if (snapshots.Count == 0)
            return;

        var json = JsonSerializer.Serialize(snapshots);
        var bytes = Encoding.UTF8.GetBytes(json);

        foreach (var socket in _connections.GetAll())
        {
            if (socket.State != WebSocketState.Open)
                continue;

            socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }
}

// --------------------
// Domain models
// --------------------

public class GameState
{
    public ConcurrentDictionary<Guid, Player> Players { get; } = new();
}

public class Player
{
    public Guid Id { get; init; }

    // Authoritative state
    public float X { get; set; }
    public float Y { get; set; }

    // Buffered input
    public float InputX { get; set; }
    public float InputY { get; set; }
}

public record PlayerInput(float X, float Y);
public record PlayerSnapshot(Guid Id, float X, float Y);

// --------------------
// Connection tracking
// --------------------

public class ConnectionManager
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

    public void Add(Guid playerId, WebSocket socket)
    {
        _sockets[playerId] = socket;
    }

    public void Remove(Guid playerId)
    {
        _sockets.TryRemove(playerId, out _);
    }

    public IEnumerable<WebSocket> GetAll()
    {
        return _sockets.Values;
    }
}

// --------------------
// Shared serialization config
// --------------------

public static class Serialization
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
