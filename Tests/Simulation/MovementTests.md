## Test 1: AppliesInput_MovesPlayerOneTick

### Purpose
Verifies that a single simulation tick applies player input correctly and
produces a deterministic change in position.

### Scenario
- Starting position: (0, 0)
- Input: (1, 0)
- Speed per tick: 0.1

### Expected Result
- X increases by 0.1
- Y remains unchanged

### What This Test Protects
- Correct scaling of input by movement speed
- Independence of X and Y movement axes
- Deterministic, tick-based movement behavior

### Notes
This test operates on the pure movement function and does not involve:
- networking
- timing logic
- player state mutation
- snapshots or serialization

If this test fails, authoritative movement behavior has changed and must be
reviewed before continuing development.


---


## Test 2: DiagonalInput_IsClampedToMaxSpeed

### Purpose
Verifies that diagonal movement input is clamped so that the player does not
move faster than the maximum allowed speed.

### Scenario
- Starting position: (0, 0)
- Input: (1, 1)
- Speed per tick: 0.1

### Expected Result
- Input is normalized
- X increases by `speedPerTick / sqrt(2)`
- Y increases by `speedPerTick / sqrt(2)`

### What This Test Protects
- Prevention of diagonal speed exploits
- Enforcement of maximum movement speed
- Correct normalization of input vectors
- Deterministic movement behavior

### Notes
This test operates on the pure movement simulation function and does not
involve:
- networking
- tick timing
- snapshots
- player state mutation

If this test fails, speed clamping behavior has changed and must be reviewed
before continuing development.


---


## Test 3: MultipleTicks_AccumulateLinearly

### Purpose
Verifies that repeated application of movement over multiple simulation ticks
produces linear, deterministic position changes.

### Scenario
- Starting position: (0, 0)
- Input: (1, 0)
- Speed per tick: 0.1
- Ticks: 10

### Expected Result
- X increases to 1.0
- Y remains unchanged

### What This Test Protects
- Absence of hidden movement state
- Deterministic behavior across ticks
- Correct accumulation of movement over time
