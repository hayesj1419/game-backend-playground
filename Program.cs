using GameServer.Game;
using GameServer.Networking;
// --------------------
// App setup
// --------------------

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});

// --------------------
// Authoritative systems
// --------------------

var gameState = new GameState();
var connections = new ConnectionManager();
var gameLoop = new GameLoop(gameState, connections, tickRate: 2);

// Start game loop
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

    // Server-owned identity
    var playerId = Guid.NewGuid();
    var player = new Player { Id = playerId };

    gameState.Players[playerId] = player;
    connections.Add(playerId, socket);

    Console.WriteLine($"Player connected: {playerId}");
    
    // Hand off to networking layer
    await WebSocketHandler.HandleAsync(socket, playerId, gameState);

    // Cleanup on disconnect
    connections.Remove(playerId);
    gameState.Players.TryRemove(playerId, out _);

    Console.WriteLine($"Player disconnected: {playerId}");
});

app.MapGet("/", () => "Game server running");

app.Run();
