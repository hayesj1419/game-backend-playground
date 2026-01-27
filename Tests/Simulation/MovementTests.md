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
