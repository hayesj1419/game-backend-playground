using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace GameServer.Networking;

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