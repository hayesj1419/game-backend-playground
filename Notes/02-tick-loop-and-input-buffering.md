# Tick Loop + Input Buffering

## 1. What problem does this solve?

Clients send input at unpredictable times and speeds.
If input directly changes game state, faster clients (lower latency, higher packet rate)
gain unfair advantages and the simulation becomes nondeterministic.

The tick loop centralizes *when* the game state is allowed to change.
Input buffering separates *intent* from *outcome*.

---

## 2. Core Idea

Clients send intent asynchronously; the server applies that intent at fixed time intervals.

---

## 3. How the System Works

- The server runs a fixed tick loop (e.g., 30 ticks per second)
- WebSocket handlers receive input and store it on the player (`InputX`, `InputY`)
- The tick loop reads the latest buffered input once per tick
- Player position (`X`, `Y`) is updated only inside the tick loop

Networking never directly mutates authoritative state.

---

## 4. Why Input Is Buffered (Not Applied Immediately)

If input were applied on receive:
- Faster packet senders would move more
- Packet spam would become a speed hack
- Lag would cause teleporting or desync
- The server would lose control of time

Buffering input ensures:
- Fairness across clients
- Deterministic simulation
- Server authority over movement

---

## 5. What Breaks If This Is Removed

- Applying input immediately → unfair movement
- Removing the tick loop → simulation tied to network timing
- Letting clients set `X/Y` → cheating and state corruption
- Clearing input every tick → requires clients to spam packets

---

## 6. Where This Runs (Context)

- Tick loop:
  - Background task
  - Time-driven
  - Deterministic

- WebSocket handler:
  - Async
  - Event-driven
  - Collects intent only

These two systems must remain separate.

---

## 7. Real Game Example

Old School RuneScape:
- Runs on a fixed server tick (~600ms)
- Player clicks send intent
- Movement and actions occur only on ticks
- Faster clicking does not increase speed

This server follows the same model at a smaller scale.

---

## 8. Key Terms to Remember

- Authoritative server
- Fixed timestep
- Input buffering
- Determinism
- Intent vs state
