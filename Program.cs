using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using GameServer.Dtos;
using GameServer.Game;
using GameServer.Networking;
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

    await WebSocketHandler.HandleAsync(socket, playerId, gameState);

    connections.Remove(playerId);
    gameState.Players.TryRemove(playerId, out _);

    Console.WriteLine($"Player disconnected: {playerId}");
});

app.MapGet("/", () => "Game server running");

app.Run();
