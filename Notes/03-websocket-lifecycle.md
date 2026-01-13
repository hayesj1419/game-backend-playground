# WebSocket Lifecycle

## 1. What problem does this solve?

Multiplayer games require persistent, bidirectional communication.
HTTP request/response is too short-lived for real-time interaction.

WebSockets allow the server to maintain a live connection to each client.

---

## 2. Core Idea

Each connected client has a persistent socket that represents a live session.

---

## 3. How It Works

- Client connects via WebSocket
- Server accepts the socket
- Server creates a session-scoped player identity
- Input is received asynchronously
- Socket remains open until disconnect
- Cleanup happens on close

The socket lifecycle maps directly to player presence.

---

## 4. What Breaks If This Is Missing

- No real-time communication
- No way to push updates to clients
- Input must be polled
- Multiplayer becomes impractical

---

## 5. Where This Runs (Context)

- Event-driven
- Async I/O
- Independent of the tick loop
- Runs concurrently with the simulation

---

## 6. Real Game Example

Old School RuneScape:
- Each logged-in player maintains a persistent connection
- Actions are sent over that connection
- Disconnect immediately removes the player from the world
