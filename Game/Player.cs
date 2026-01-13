namespace GameServer.Game;

public class Player
{
    public Guid Id { get; init; }

    //Authoritative state
    public float X { get; set; }
    public float Y { get; set; }

    //Buffered input
    public float InputX { get; set; }
    public float InputY { get; set; }
}