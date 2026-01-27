using System;

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
        // Compute input magnitude
        var magnitude = MathF.Sqrt(inputX * inputX + inputY * inputY);

        if (magnitude > 1f)
            {
                // Normalize input to prevent faster diagonal movement
                inputX /= magnitude;
                inputY /= magnitude;
            }

        x += inputX * speedPerTick;
        y += inputY * speedPerTick;

        return (x, y);
    }
}
}