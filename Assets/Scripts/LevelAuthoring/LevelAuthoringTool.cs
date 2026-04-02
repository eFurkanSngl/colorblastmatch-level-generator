using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Editor-focused level authoring workflow:
// 1) Select LevelData
// 2) Generate Cells (via GridManager + CellPool)
// 3) Paint grid map in inspector (Empty / Tile / Obstacle)
// 4) Generate Obstacle
// 5) Save To LevelData + Prefab
[ExecuteAlways]
public class LevelAuthoringTool : SerializedMonoBehaviour
{
    private enum AuthoringCellKind
    {
        Empty = 0,
        Tile = 1,
        Obstacle = 2
    }

    [Header("Level Source")]
    [SerializeField] private LevelData _levelData;

    [Header("Dependencies")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private CellPool _cellPool;

    [Header("Prefabs")]
    [SerializeField] private GameObject _obstaclePrefab;

    [Header("Scene Roots")]
    [Tooltip("All generated nodes are kept under LevelParent so prefab layout is consistent.")]
    [SerializeField] private Transform _levelRoot;
    [SerializeField] private Transform _cellsRoot;
    [SerializeField] private Transform _obstaclesRoot;

    [Header("Save")]
    [SerializeField] private string _levelPrefabFolder = "Assets/Prefabs/Levels";
    [SerializeField] private bool _syncMapFromSceneOnSave = true;

    [InfoBox("Grid Map: click each cell to cycle Empty -> Tile -> Obstacle.")]
    [OdinSerialize]
    [TableMatrix(SquareCells = true, ResizableColumns = false, DrawElementMethod = nameof(DrawGridCell))]
    private AuthoringCellKind[,] _gridMap;

    [Header("Debug")]
    [SerializeField] private int _generatedCells;
    [SerializeField] private int _generatedObstacles;

    [Button("Generate Cells", ButtonSizes.Large), DisableInPlayMode]
    [ContextMenu("Generate Cells")]
    public void GenerateCells()
    {
        if (!ValidateCoreInputs(requireGridManager: true))
        {
            return;
        }

        EnsureRoots();
        EnsureGridMapInitialized();

        ClearGeneratedVisuals();

        _gridManager.GenerateCell();
        ApplyGridMaskToGeneratedCells();
        _generatedCells = CountActiveCellsInScene();
        _generatedObstacles = 0;
        MarkSceneDirty();
    }

    [Button("Generate Obstacle", ButtonSizes.Medium), DisableInPlayMode]
    [ContextMenu("Generate Obstacle")]
    public void GenerateObstacles()
    {
        if (!ValidateCoreInputs(requireGridManager: false))
        {
            return;
        }

        EnsureRoots();
        EnsureGridMapInitialized();

        ClearChildren(_obstaclesRoot);

        if (_obstaclePrefab == null)
        {
            _generatedObstacles = 0;
            return;
        }

        int count = 0;
        for (int x = 0; x < _levelData.GridX; x++)
        {
            for (int y = 0; y < _levelData.GridY; y++)
            {
                if (_gridMap[x, y] != AuthoringCellKind.Obstacle)
                {
                    continue;
                }

                GameObject obstacle = InstantiatePrefab(_obstaclePrefab, _obstaclesRoot);
                obstacle.transform.localPosition = new Vector3(x, y, 0f);
                obstacle.name = $"Obstacle_{x}_{y}";
                count++;
            }
        }

        _generatedObstacles = count;
        MarkSceneDirty();
    }

    [Button("Clear Generated", ButtonSizes.Medium), DisableInPlayMode, GUIColor(0.95f, 0.5f, 0.3f)]
    [ContextMenu("Clear Generated")]
    public void ClearGeneratedVisuals()
    {
        EnsureRoots();

        if (_cellPool != null)
        {
            CellView[] cells = _cellsRoot.GetComponentsInChildren<CellView>(false);
            for (int i = 0; i < cells.Length; i++)
            {
                _cellPool.ReturnCell(cells[i]);
            }
        }
        else
        {
            ClearChildren(_cellsRoot);
        }

        ClearChildren(_obstaclesRoot);
        ClearObstacleMarksOnMap();

        _generatedCells = 0;
        _generatedObstacles = 0;
        MarkSceneDirty();
    }

    [Button("Save To LevelData + Prefab", ButtonSizes.Large), DisableInPlayMode, GUIColor(0.35f, 0.8f, 0.35f)]
    [ContextMenu("Save To LevelData + Prefab")]
    public void SaveLevel()
    {
        if (!ValidateCoreInputs(requireGridManager: false))
        {
            return;
        }

        EnsureRoots();
        EnsureGridMapInitialized();

        if (_syncMapFromSceneOnSave)
        {
            SyncMapFromSceneCells();
        }

        List<CellCoordinateData> activeCells = new List<CellCoordinateData>();
        List<ObstaclePlacementData> obstacles = new List<ObstaclePlacementData>();

        for (int x = 0; x < _levelData.GridX; x++)
        {
            for (int y = 0; y < _levelData.GridY; y++)
            {
                AuthoringCellKind kind = _gridMap[x, y];
                if (kind == AuthoringCellKind.Empty)
                {
                    continue;
                }

                activeCells.Add(new CellCoordinateData { x = x, y = y });

                if (kind == AuthoringCellKind.Obstacle)
                {
                    obstacles.Add(new ObstaclePlacementData
                    {
                        x = x,
                        y = y,
                        obstacleType = ObstacleType.WoodCrate
                    });
                }
            }
        }

        List<TilePlacementData> tilePlacements = new List<TilePlacementData>();
        GameObject savedPrefab = SaveCurrentLayoutAsPrefab() ?? _levelData.LevelPrefab;

        _levelData.ApplyAuthoringData(savedPrefab, _levelData.TileSeed, activeCells, obstacles, tilePlacements);

#if UNITY_EDITOR
        EditorUtility.SetDirty(_levelData);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif

        _generatedCells = activeCells.Count;
        _generatedObstacles = obstacles.Count;

        Debug.Log($"LevelAuthoringTool: Saved '{_levelData.name}' | Active={activeCells.Count}, Obstacles={obstacles.Count}");
    }

    private bool ValidateCoreInputs(bool requireGridManager)
    {
        if (_levelData == null)
        {
            Debug.LogWarning("LevelAuthoringTool: LevelData reference is missing.");
            return false;
        }

        if (_levelData.GridX <= 0 || _levelData.GridY <= 0)
        {
            Debug.LogWarning("LevelAuthoringTool: GridX and GridY must be greater than zero.");
            return false;
        }

        if (requireGridManager)
        {
            if (_gridManager == null)
            {
                _gridManager = GetComponent<GridManager>();
            }

            if (_gridManager == null)
            {
                Debug.LogWarning("LevelAuthoringTool: GridManager bulunamadi.");
                return false;
            }
        }

        return true;
    }

    private void EnsureGridMapInitialized()
    {
        int width = _levelData.GridX;
        int height = _levelData.GridY;

        if (_gridMap == null || _gridMap.GetLength(0) != width || _gridMap.GetLength(1) != height)
        {
            AuthoringCellKind[,] newGrid = new AuthoringCellKind[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newGrid[x, y] = AuthoringCellKind.Tile;
                }
            }

            if (_gridMap != null)
            {
                int copyW = Math.Min(width, _gridMap.GetLength(0));
                int copyH = Math.Min(height, _gridMap.GetLength(1));
                for (int x = 0; x < copyW; x++)
                {
                    for (int y = 0; y < copyH; y++)
                    {
                        newGrid[x, y] = _gridMap[x, y];
                    }
                }
            }

            _gridMap = newGrid;
        }
    }

    // Optional bridge: if cells are manually deleted in scene, save will reflect that deletion.
    private void SyncMapFromSceneCells()
    {
        if (_cellsRoot == null || _gridMap == null)
        {
            return;
        }

        AuthoringCellKind[,] previous = (AuthoringCellKind[,])_gridMap.Clone();
        for (int x = 0; x < _levelData.GridX; x++)
        {
            for (int y = 0; y < _levelData.GridY; y++)
            {
                _gridMap[x, y] = AuthoringCellKind.Empty;
            }
        }

        CellView[] cells = _cellsRoot.GetComponentsInChildren<CellView>(false);
        for (int i = 0; i < cells.Length; i++)
        {
            int x;
            int y;
            if (cells[i].CellData != null)
            {
                x = cells[i].CellData.gridX;
                y = cells[i].CellData.gridY;
            }
            else
            {
                Vector3 lp = cells[i].transform.localPosition;
                x = Mathf.RoundToInt(lp.x);
                y = Mathf.RoundToInt(lp.y);
            }

            if (x < 0 || y < 0 || x >= _levelData.GridX || y >= _levelData.GridY)
            {
                continue;
            }

            _gridMap[x, y] = previous[x, y] == AuthoringCellKind.Obstacle
                ? AuthoringCellKind.Obstacle
                : AuthoringCellKind.Tile;
        }
    }

    private bool IsCellActive(int x, int y)
    {
        if (_gridMap == null)
        {
            return false;
        }

        if (x < 0 || y < 0 || x >= _gridMap.GetLength(0) || y >= _gridMap.GetLength(1))
        {
            return false;
        }

        return _gridMap[x, y] != AuthoringCellKind.Empty;
    }

    private void ApplyGridMaskToGeneratedCells()
    {
        if (_cellsRoot == null || _cellPool == null)
        {
            return;
        }

        CellView[] cells = _cellsRoot.GetComponentsInChildren<CellView>(false);
        for (int i = 0; i < cells.Length; i++)
        {
            CellData data = cells[i].CellData;
            if (data == null || !IsCellActive(data.gridX, data.gridY))
            {
                _cellPool.ReturnCell(cells[i]);
            }
        }
    }

    private int CountActiveCellsInScene()
    {
        if (_cellsRoot == null)
        {
            return 0;
        }

        return _cellsRoot.GetComponentsInChildren<CellView>(false).Length;
    }

    private void ClearObstacleMarksOnMap()
    {
        if (_gridMap == null)
        {
            return;
        }

        for (int x = 0; x < _gridMap.GetLength(0); x++)
        {
            for (int y = 0; y < _gridMap.GetLength(1); y++)
            {
                if (_gridMap[x, y] == AuthoringCellKind.Obstacle)
                {
                    _gridMap[x, y] = AuthoringCellKind.Tile;
                }
            }
        }
    }

    private void EnsureRoots()
    {
        if (_levelRoot == null)
        {
            Transform existing = transform.Find("LevelParent");
            if (existing != null)
            {
                _levelRoot = existing;
            }
            else
            {
                GameObject root = new GameObject("LevelParent");
                root.transform.SetParent(transform, false);
                _levelRoot = root.transform;
            }
        }

        _cellsRoot = EnsureChildRoot(_cellsRoot, "Cells");
        _obstaclesRoot = EnsureChildRoot(_obstaclesRoot, "Obstacles");
    }

    private Transform EnsureChildRoot(Transform root, string childName)
    {
        if (root != null)
        {
            return root;
        }

        Transform existing = _levelRoot.Find(childName);
        if (existing != null)
        {
            return existing;
        }

        GameObject child = new GameObject(childName);
        child.transform.SetParent(_levelRoot, false);
        return child.transform;
    }

    private GameObject InstantiatePrefab(GameObject prefab, Transform parent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject editorPrefab = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (editorPrefab != null)
            {
                return editorPrefab;
            }
        }
#endif

        return Instantiate(prefab, parent);
    }

    private void ClearChildren(Transform root)
    {
        if (root == null)
        {
            return;
        }

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform child = root.GetChild(i);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
#else
            Destroy(child.gameObject);
#endif
        }
    }

    private GameObject SaveCurrentLayoutAsPrefab()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return _levelData.LevelPrefab;
        }

        if (string.IsNullOrWhiteSpace(_levelPrefabFolder))
        {
            _levelPrefabFolder = "Assets/Prefabs/Levels";
        }

        EnsureAssetFolder(_levelPrefabFolder);
        string prefabPath = $"{_levelPrefabFolder}/{_levelData.name}.prefab";
        return PrefabUtility.SaveAsPrefabAsset(_levelRoot.gameObject, prefabPath);
#else
        return null;
#endif
    }

#if UNITY_EDITOR
    private static void EnsureAssetFolder(string folderPath)
    {
        string normalized = folderPath.Replace("\\", "/");
        string[] segments = normalized.Split('/');

        if (segments.Length == 0 || segments[0] != "Assets")
        {
            throw new InvalidOperationException("Level prefab folder must start with 'Assets'.");
        }

        string current = segments[0];
        for (int i = 1; i < segments.Length; i++)
        {
            string next = $"{current}/{segments[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, segments[i]);
            }

            current = next;
        }
    }

    private void MarkSceneDirty()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }

    private static AuthoringCellKind DrawGridCell(Rect rect, AuthoringCellKind value)
    {
        Color previous = GUI.backgroundColor;
        GUI.backgroundColor = GetGridColor(value);

        if (GUI.Button(rect, GUIContent.none))
        {
            value = Next(value);
        }

        GUI.backgroundColor = previous;
        return value;
    }

    private static AuthoringCellKind Next(AuthoringCellKind value)
    {
        switch (value)
        {
            case AuthoringCellKind.Empty:
                return AuthoringCellKind.Tile;
            case AuthoringCellKind.Tile:
                return AuthoringCellKind.Obstacle;
            default:
                return AuthoringCellKind.Empty;
        }
    }

    private static Color GetGridColor(AuthoringCellKind value)
    {
        switch (value)
        {
            case AuthoringCellKind.Empty:
                return new Color(0.35f, 0.35f, 0.35f, 1f);
            case AuthoringCellKind.Tile:
                return new Color(0.2f, 0.65f, 0.95f, 1f);
            case AuthoringCellKind.Obstacle:
                return new Color(0.68f, 0.49f, 0.32f, 1f);
            default:
                return Color.white;
        }
    }
#else
    private void MarkSceneDirty() { }
#endif
}
