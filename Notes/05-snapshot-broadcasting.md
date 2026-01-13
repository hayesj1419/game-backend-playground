# Snapshot Broadcasting

## 1. What problem does this solve?

Clients need a consistent, authoritative view of the game world.
Without server snapshots, each client would drift or rely on guesses.

---

## 2. Core Idea

The server periodically sends the full authoritative state to all connected clients.

---

## 3. How It Works

- Tick loop advances authoritative game state
- A snapshot DTO is created from server state
- Snapshots are serialized and broadcast to all active connections
- Clients render based on received snapshots

---

## 4. What Breaks If This Is Missing

- Clients diverge
- No correction for prediction errors
- No shared world state
- Multiplayer is impossible

---

## 5. Tradeoffs

- Simple but bandwidth-heavy
- No interpolation yet
- No delta compression yet

---

## 6. Real Game Example

World of Warcraft:
- Server sends frequent authoritative updates
- Client corrects position if it drifts
- Server state always wins
