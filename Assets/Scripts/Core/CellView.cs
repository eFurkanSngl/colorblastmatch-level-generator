using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    private CellData _cellData;

    public CellData CellData => _cellData;

    [SerializeField] private GameObject _cellView;
    public void Init(CellData celldata)
    {
        _cellData = celldata;
    }
}
