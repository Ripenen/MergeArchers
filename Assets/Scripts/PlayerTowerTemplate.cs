using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTowerTemplate : TowerTemplate, IWithId
{
    [SerializeField] private Transform _aimCameraPosition;
    [SerializeField] private Transform _mergeCameraPosition;
    [SerializeField] private int _id;
    [SerializeField] private Sprite _icon;
    
    [SerializeField] private GameObject _twoRows;
    [SerializeField] private GameObject _threeRows;
    [SerializeField] private GameObject _fourRows;

    public Transform AimCameraPosition => _aimCameraPosition;
    public Transform MergeCameraPosition => _mergeCameraPosition;

    public int Id => _id;

    public Sprite Icon => _icon;

    public void ActivateRows(int rows)
    {
        _twoRows.SetActive(false);
        _threeRows.SetActive(false);
        _fourRows.SetActive(false);

        GameObject active = _twoRows;
        
        switch (rows)
        {
            case 2:
                active = _twoRows;
                break;
            case 3:
                active = _threeRows;
                break;
            case 4:
                active = _fourRows;
                break;
        }
        
        active.SetActive(true);

        var cells = active.GetComponentsInChildren<ArcherPlaceCell>(false);
        
        if(_cells is not null)
            for (int i = 0; i < _cells.Length; i++)
                cells[i].Apply(_cells[i]);

        _cells = cells;
    }

    public void CopyPlaceCellsDataTo(PlayerTowerTemplate to)
    {
        for (int i = 0; i < to._cells.Length; i++)
        {
            to._cells[i].Apply(_cells[i]);
        }
    }

    public void SetCellsData(List<ArcherCell> cells)
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            if(cells[i].Item1)
                _cells[i].MakeBusy(cells[i].Item2);
            else
                _cells[i].MakeFree();
        }
    }
}