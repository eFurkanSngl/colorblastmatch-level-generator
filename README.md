# ColorBlastMatch Level Generator

Unity-based level authoring toolkit extracted from the ColorBlastMatch project.

This repo contains only the **level generator / authoring side**:
- Grid cell generation
- Obstacle authoring
- Level prefab save flow
- Level data serialization for active cells and obstacles

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

## Current Design

- **Editor/Authoring**:
  - Generate cells
  - Paint map (`Empty / Tile / Obstacle`)
  - Generate obstacles
  - Save level layout to prefab + level data
- **Runtime**:
  - Tiles are intended to be spawned via pool at runtime (not authored as prefab content)

## Requirements

- Unity 2021+ (or newer)
- Odin Inspector (for authoring UI attributes)

## Typical Workflow

1. Assign a `LevelData` asset in `LevelAuthoringTool`.
2. Generate cells.
3. Mark obstacle positions.
4. Generate obstacles.
5. Save to prefab + data.
6. At runtime, load level prefab and spawn tiles from pool.

## Notes

- This repository is intentionally focused on level-generator modules.
- Tile spawn/runtime gameplay systems are planned as a separate runtime module.
https://x.com/lanuykumvar/status/2039583942131535981?s=20
