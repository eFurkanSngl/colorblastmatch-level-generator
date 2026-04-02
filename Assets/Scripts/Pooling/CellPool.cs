using System.Collections.Generic;
using UnityEngine;

public class CellPool : MonoBehaviour
{
    [SerializeField] private CellView _cellPrefab;
    [SerializeField] private int _cellCount = 10;
    [SerializeField] private Transform _cellRoot;

    private readonly Queue<CellView> _cells = new Queue<CellView>();

    private void Awake()
    {
        Transform parent = _cellRoot != null ? _cellRoot : transform;

        for (int i = 0; i < _cellCount; i++)
        {
            CellView cell = Instantiate(_cellPrefab, parent);
            cell.gameObject.SetActive(false);
            _cells.Enqueue(cell);
        }
    }

    public CellView GetCells()
    {
        Transform parent = _cellRoot != null ? _cellRoot : transform;
        CellView cell = null;

        while (_cells.Count > 0 && cell == null)
        {
            cell = _cells.Dequeue();
        }

        if (cell == null)
        {
            cell = Instantiate(_cellPrefab, parent);
        }

        cell.transform.SetParent(parent, false);
        cell.gameObject.SetActive(true);
        return cell;
    }

    public void ReturnCell(CellView cell)
    {
        if (cell == null)
        {
            return;
        }

        cell.Init(null);
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            cell.gameObject.SetActive(false);
            _cells.Enqueue(cell);
            return;
        }
#endif
        cell.transform.SetParent(transform, false);
        cell.gameObject.SetActive(false);
        _cells.Enqueue(cell);
    }
}
