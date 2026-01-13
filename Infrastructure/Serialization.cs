using System.Text.Json;

namespace GameServer.Infrastructure;

public static class Serialization
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
