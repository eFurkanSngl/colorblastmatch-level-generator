using System;
using System.Collections.Generic;
using Assets.Scripts.Enum;
using UnityEngine;

[CreateAssetMenu(fileName ="GameLevelData",menuName ="LevelData/Installers")]
public class LevelData : ScriptableObject
{
    [Header("Legacy")]
    [SerializeField] private int _targetCount;

    [Header("Core")]
    [SerializeField] private int _moveCount;
    [SerializeField] private int _gridX;
    [SerializeField] private int _gridY;
    [SerializeField] private GameObject _levelPrefab;

    [Header("Authoring Data")]
    [SerializeField] private List<LevelTargetData> _targets = new List<LevelTargetData>();
    [SerializeField] private List<CellCoordinateData> _activeCells = new List<CellCoordinateData>();
    [SerializeField] private List<ObstaclePlacementData> _obstacles = new List<ObstaclePlacementData>();
    [SerializeField] private List<TilePlacementData> _tiles = new List<TilePlacementData>();
    [SerializeField] private int _tileSeed;


    public int TargetCount => _targetCount;
    public int MoveCount => _moveCount;
    public int GridX => _gridX;
    public int GridY => _gridY;
    public GameObject LevelPrefab => _levelPrefab;
    public int TileSeed => _tileSeed;
    public IReadOnlyList<LevelTargetData> Targets => _targets;
    public IReadOnlyList<CellCoordinateData> ActiveCells => _activeCells;
    public IReadOnlyList<ObstaclePlacementData> Obstacles => _obstacles;
    public IReadOnlyList<TilePlacementData> Tiles => _tiles;

    public void ApplyAuthoringData(
        GameObject levelPrefab,
        int tileSeed,
        List<CellCoordinateData> activeCells,
        List<ObstaclePlacementData> obstacles,
        List<TilePlacementData> tiles)
    {
        _levelPrefab = levelPrefab;
        _tileSeed = tileSeed;
        _activeCells = activeCells != null ? new List<CellCoordinateData>(activeCells) : new List<CellCoordinateData>();
        _obstacles = obstacles != null ? new List<ObstaclePlacementData>(obstacles) : new List<ObstaclePlacementData>();
        _tiles = tiles != null ? new List<TilePlacementData>(tiles) : new List<TilePlacementData>();
    }

}

[Serializable]
public struct CellCoordinateData
{
    public int x;
    public int y;
}

[Serializable]
public struct LevelTargetData
{
    public BlockColorType colorType;
    public int count;
}

[Serializable]
public struct ObstaclePlacementData
{
    public int x;
    public int y;
    public ObstacleType obstacleType;
}

[Serializable]
public struct TilePlacementData
{
    public int x;
    public int y;
    public BlockColorType colorType;
}

public enum ObstacleType
{
    None = 0,
    WoodCrate = 1,
    StrongCrate = 2,
    ChainLock = 3
}
