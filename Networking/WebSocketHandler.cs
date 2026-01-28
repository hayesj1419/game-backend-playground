using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using GameServer.Dtos;
using GameServer.Game;
using GameServer.Infrastructure;

namespace GameServer.Networking;

public static class WebSocketHandler
{
    public static async Task HandleAsync(
        WebSocket socket,
        Guid playerId,
        GameState gameState)
    {
        var welcome = new
        {
            type = "welcome",
            playerId = playerId.ToString()
        };

        var welcomeJson = JsonSerializer.Serialize(welcome, Serialization.JsonOptions);
        var welcomeBytes = Encoding.UTF8.GetBytes(welcomeJson);
        
        await socket.SendAsync(
            new ArraySegment<byte>(welcomeBytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
        
        Console.WriteLine($"[SERVER] Sent welcome to {playerId}");

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

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch
            {
                continue;
            }

            if(!doc.RootElement.TryGetProperty("type", out var typeElement))
                continue;
            var type = typeElement.GetString();
            if(type == "input")
            {
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
    }
}
}