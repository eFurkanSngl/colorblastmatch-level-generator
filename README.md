# Level Generator

This Unity module provides a clean editor-side workflow for creating match-style level layouts.
This repository contains only the **level authoring pipeline**.

## Scope

- Generates grid cells
- Lets you paint a grid map in Inspector (`Empty / Tile / Obstacle`)
- Generates obstacles from painted cells
- Saves final layout to `LevelData + Prefab`
- Prepares data for runtime systems

## Important Note About Tiles

Tile authoring is **not included** in this tool by design.

- `Cells + Obstacles` are authored in editor and saved to prefab/data.
- `Tiles` should be spawned at runtime (usually via pool/spawner).

This keeps runtime match/destroy/gravity systems modular and easier to maintain.

## Included Scripts

- `Assets/Scripts/LevelAuthoring/LevelAuthoringTool.cs`
- `Assets/Scripts/LevelAuthoring/Editor/LevelAuthoringWindow.cs`
- `Assets/Scripts/Core/GridManager.cs`
- `Assets/Scripts/Pooling/CellPool.cs`
- `Assets/Scripts/Core/CellView.cs`
- `Assets/Scripts/CellData/CellData.cs`
- `Assets/Scripts/RuntimeData/GridruntimeData.cs`
- `Assets/Scripts/LevelData/LevelData.cs`
- `Assets/Scripts/Enum/*`

## Requirements

- Unity 2021+ (or newer)
- Odin Inspector

## Scene Setup

1. Create a `GenerateGrid` GameObject in scene.
2. Add these components to the same object:
   - `GridManager`
   - `LevelAuthoringTool`
3. Create a `PoolManager` object and add `CellPool`.
4. In `LevelAuthoringTool`, assign:
   - `Level Data`
   - `Grid Manager`
   - `Cell Pool`
   - `Level Root` (`LevelParent`)
   - `Cells Root`
   - `Obstacles Root`
   - `Obstacle Prefab`
5. In `CellPool`, assign:
   - `Cell Prefab`
   - `Cell Root`

## Step-by-Step Workflow

1. Select the target `LevelData` in `LevelAuthoringTool`.
2. Click `Generate Cells`.
3. Paint cells in grid map (`Empty / Tile / Obstacle`).
4. Click `Generate Obstacle`.
5. Review scene layout.
6. Click `Save To LevelData + Prefab`.
7. Repeat with the next `LevelData` for next level.

## Recommended Runtime Flow

1. Load level prefab.
2. Read level data (`active cells`, `obstacles`, seed if needed).
3. Spawn tiles at runtime using your tile pool/spawner.

## Demo (GIF / Video)

Add GIF:

```md
![Level Generator Demo](docs/media/level-generator.gif)
```

Add video link:

```md
[Demo Video](docs/media/level-generator-demo.mp4)
```

Example screenshot:

<img width="534" height="1115" alt="image" src="https://github.com/user-attachments/assets/c7287a02-3c6c-49eb-a3f4-b64171e4c082" />

https://github.com/user-attachments/assets/8d241cb6-fdd5-4b9d-b23f-3d7cd3ae6ba8


