# Tick IDs

## Problem

Snapshots without ordering cannot be validated or reconciled.

---

## Core Idea

Each snapshot includes a monotonically increasing server tick ID.

---

## How It Works

- Server increments a tick counter every simulation step
- Tick ID is attached to each snapshot batch
- Clients use tick IDs to order and validate snapshots

---

## What Breaks Without It

- Out-of-order updates
- Impossible desync debugging
- Broken interpolation
- Client cannot discard stale state

---

## Real Game Example

FPS servers (CS/Valorant):
- Server tick IDs anchor reconciliation and hit validation
