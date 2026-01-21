# Snapshot Throttling

## Problem

Sending snapshots every tick wastes bandwidth and limits scalability.

---

## Core Idea

Simulation rate and network update rate are separate concerns.

---

## How It Works

- Server simulates every tick
- Server only broadcasts snapshots every N ticks
- Tick IDs preserve ordering
- Clients interpolate between snapshots

---

## What This Enables

- Lower bandwidth usage
- Better scaling with more players
- Smoother client visuals
- Foundation for interpolation

---

## Real Game Example

World of Warcraft:
- Server simulates continuously
- Position updates are throttled
- Clients interpolate movement
- Server corrections override client guesses
