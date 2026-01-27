using System.Diagnostics;
using System.Text;
using System.Text.Json;
using GameServer.Dtos;
using GameServer.Infrastructure;
using GameServer.Networking;
using GameServer.Game.Simulation;
using Microsoft.AspNetCore.Http.Json;

namespace GameServer.Game;

public class GameLoop
{
    private readonly GameState _state;
    private readonly int _snapshotEveryNTicks;
    private readonly ConnectionManager _connections;
    private readonly TimeSpan _tickInterval;
    private long _currentTick = 0;

    public GameLoop(
        GameState state, 
        ConnectionManager connections, 
        int tickRate,
        int snapshotRate)
    {
        _state = state;
        _connections = connections;
        _tickInterval = TimeSpan.FromMilliseconds(1000.0 / tickRate);
        
        _snapshotEveryNTicks = tickRate / snapshotRate;
    }

    public void Start()
{
    Console.WriteLine("GameLoop started");
    Task.Run(async () =>
    {
        var stopwatch = Stopwatch.StartNew();
        var nextTickTime = stopwatch.Elapsed;

        while (true)
        {
            await TickAsync();

            nextTickTime += _tickInterval;
            var delay = nextTickTime - stopwatch.Elapsed;

            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }
            else
            {
                // We are behind; skip delay to catch up
                nextTickTime = stopwatch.Elapsed;
            }
        }
    });
}


    private async Task TickAsync()
    {
        _currentTick++;

        const float speedPerTick = 0.1f;
        var players = new List<PlayerSnapshot>();

        foreach (var player in _state.Players.Values)
        {
            (player.X, player.Y) = MovementSystem.Apply(
                player.X,
                player.Y,
                player.InputX,
                player.InputY,
                speedPerTick
            );

            players.Add(new PlayerSnapshot(player.Id, player.X, player.Y));

            player.InputX = 0;
            player.InputY = 0;
        }

        if (_currentTick % _snapshotEveryNTicks == 0)
        {
            await Broadcast(new WorldSnapshot(_currentTick, players));
        }
    }

    private async Task Broadcast(WorldSnapshot snapshot)
    {
        var payload = new
        {
            type = "snapshot",
            tick = snapshot.Tick,
            players = snapshot.Players
        };

        var json = JsonSerializer.Serialize(payload, Serialization.JsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        foreach (var socket in _connections.GetAll())
        {
            if (socket.State != System.Net.WebSockets.WebSocketState.Open)
                continue;
            
           await socket.SendAsync(
                bytes,
                System.Net.WebSockets.WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }
}