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

        [Fact]
        public void DiagonalInput_IsClampedToMaxSpeed()
        {
            // Arrange
            float x = 0f;
            float y = 0f;

            // Diagonal input with magnitude > 1 (≈ 1.414)
            float inputX = 1f;
            float inputY = 1f;

            float speedPerTick = 0.1f;

            // Act
            (x, y) = MovementSystem.Apply(
                x,
                y,
                inputX,
                inputY,
                speedPerTick
            );
            
            // Expected normalized movement
            var expectedDelta = speedPerTick / MathF.Sqrt(2f);

            // Assert
            Assert.Equal(expectedDelta, x, 5);
            Assert.Equal(expectedDelta, y, 5);
        }
    }
}