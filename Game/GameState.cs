using System.Collections.Concurrent;

namespace GameServer.Game;

public class GameState
{
    public ConcurrentDictionary<Guid, Player> Players { get; } = new();
}