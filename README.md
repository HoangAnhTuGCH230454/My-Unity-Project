# My Unity Project — 2D Metroidvania Platformer

A 2D Metroidvania-style platformer built in Unity, inspired by *Hollow Knight*. Developed as a final-year university project.

## Overview

The player explores an interconnected world, fighting enemies, collecting upgrades, and progressing toward a climactic boss encounter with **TheBlindHuntress**.

## Features

- **Exploration-driven 2D platforming** in the Metroidvania style — interconnected areas gated by player progression/abilities
- **Combat system** with multiple enemy types:
  - `FlyingOrg` — airborne enemy
  - `Golem` — ground-based enemy
  - `Mushroom` — ground-based enemy
- **Boss fight** against TheBlindHuntress, featuring multi-stage attack patterns
- **Save/load system** with persistent progress (mana, shards, and player state)
- **Mana and Shard economy** — resource management tied to player progression and UI feedback (health, mana bars)
- **Scene-based level structure** with camera trigger transitions between areas

## Tech Stack

- **Engine:** Unity (2D)
- **Language:** C#
- **Rendering:** Custom shaders (ShaderLab / HLSL)

## Getting Started

### Prerequisites

- Unity Hub
- Unity Editor (see `ProjectSettings/ProjectVersion.txt` in this repo for the exact version used)

### Setup

1. Clone the repository:
   ```
   git clone https://github.com/HoangAnhTuGCH230454/My-Unity-Project.git
   ```
2. Open Unity Hub → **Add project from disk** → select the cloned folder.
3. Let Unity import all assets and packages (this may take a few minutes on first open).
4. Open the main/starting scene from the `Assets/Scenes` folder and press **Play** in the Unity Editor.

## Controls

- Move: `A` / `D`
- Jump: `Space`
- Attack: `Left Mouse Button`
- Dash: `Shift`
- Side Spell attack: `F`
- Up Spell attack: `W` + `F`
- Healing: Hold `F`

## Project Structure

```
Assets/           Game assets, scripts, scenes, prefabs
Packages/         Unity package manifest and dependencies
ProjectSettings/  Unity project configuration
```

## Known Issues

- Camera orthographic size is inconsistent across some scenes
- Pause Menu is being destroy on load
- Spikes in other scenes is teleporting player out of the map

## Status

Actively in development as part of a final-year degree project. Core systems (save/load, inventory, boss AI, enemy behaviors) are implemented; remaining work is focused on UI consistency across scenes and polish.

## Author

Hoang Anh Tu — GCH230454 - CO1204
