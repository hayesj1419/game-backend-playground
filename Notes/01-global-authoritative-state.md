# Global Authoritative State

## 1. What problem does this solve?

In a multiplayer game, there must be a single source of truth.
If each client owns its own state, players will desync, cheat, or diverge.

The server must own the game world.

---

## 2. Core Idea

All game state lives on the server and exists independently of clients or requests.

---

## 3. How It Works

- A long-lived GameState object is created at server startup
- This state lives for the lifetime of the process
- All systems (tick loop, networking, snapshots) read or mutate this state
- Clients never directly modify authoritative state

The server is a simulation, not a request handler.

---

## 4. What Breaks If This Is Missing

- Each client sees a different world
- Cheating becomes trivial
- State cannot be validated
- Multiplayer logic collapses

---

## 5. Where This Runs (Context)

- In-memory, server-side
- Shared across threads
- Protected by concurrency-safe structures

This state outlives any single connection.

---

## 6. Real Game Example

World of Warcraft:
- The server owns character position, health, and inventory
- Clients send requests, not state
- Logging out does not destroy the world
