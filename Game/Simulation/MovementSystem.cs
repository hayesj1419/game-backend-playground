namespace GameServer.Game.Simulation
{
    public static class MovementSystem
{
    public static (float x, float y) Apply(
        float x,
        float y,
        float inputX,
        float inputY,
        float speedPerTick)
    {
        x += inputX * speedPerTick;
        y += inputY * speedPerTick;

        return (x, y);
    }
}
}