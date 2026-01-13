namespace GameServer.Dtos;

public record WorldSnapshot(long Tick, IReadOnlyList<PlayerSnapshot> Players);