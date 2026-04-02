using Assets.Scripts.RuntimeData;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Dependency")]
    private GridruntimeData _gridRuntime;
    [SerializeField] private LevelData _levelData;
    [SerializeField] private CellPool _cellPool;

    [Header("Grid Settings")]
    private int _gridX;
    private int _gridY;

    private void Awake()
    {
        if (_levelData == null)
        {
            return;
        }

        _gridX = _levelData.GridX;
        _gridY = _levelData.GridY;
    }

    private void Start()
    {
        if (_gridRuntime != null || _levelData == null) return;

        _gridRuntime = new GridruntimeData(_gridX, _gridY);
    }

    public void GenerateCell()
    {
     
        _gridRuntime = new GridruntimeData(_gridX, _gridY);

        for (int i = 0; i < _gridX; i++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                Vector3 pos = new Vector3(i, y, transform.position.z);
                CellView cellObject = _cellPool.GetCells();
                cellObject.transform.position = pos;

                CellData cell = _gridRuntime.GetCell(i, y);
                cellObject.Init(cell);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_levelData != null)
        {
            _gridX = _levelData.GridX;
            _gridY = _levelData.GridY;
        }

        for (int i = 0; i < _gridX; i++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                Vector3 pos = new Vector3(transform.position.x + i, transform.position.y + y, transform.position.z);
                Gizmos.DrawWireCube(pos, new Vector3(1, 1, 1));
            }
        }
    }
#endif
}
