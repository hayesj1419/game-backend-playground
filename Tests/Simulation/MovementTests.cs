using Xunit;
using GameServer.Game.Simulation;

namespace GameServer.Tests.Simulation
{
    public class MovementTests
    {
        [Fact]
        public void AppliesInput_MovesPlayerOneTick()
        {
            // Arrange
            float x = 0f;
            float y = 0f;
            float inputX = 1f;
            float inputY = 0f;
            float speedPerTick = 0.1f;

            // Act
            (x, y) = MovementSystem.Apply(
                x,
                y,
                inputX,
                inputY,
                speedPerTick
            );

            // Assert
            Assert.Equal(0.1f, x, 5);
            Assert.Equal(0f, y, 5);
        }
    }
}