## Session: <1/13/2026>

### What I worked on
- Refactored server into Game / Networking / Infrastructure
- Fixed tick timing bug (microseconds vs milliseconds)
- Cleaned Program.cs into composition root

### Bugs I hit
- Runaway tick loop due to wrong TimeSpan unit
- Async send without pacing
- WebSocket middleware vs services confusion

### What I understand better now
- Tick rate vs movement speed
- Backpressure in async systems
- Why file structure follows dependency direction

### Next step
- Snapshot throttling vs tick rate

Tomorrow start by:
- Reviewing snapshot broadcasting
- Adding snapshot throttling (network ≠ tick rate)
