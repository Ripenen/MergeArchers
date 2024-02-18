using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerTemplate : MonoBehaviour
{
    [SerializeField] private Key _key;
    
    [HideInInspector] public Balloon _balloon;
    
    protected ArcherPlaceCell[] _cells;

    public IEnumerable<ArcherPlaceCell> Cells => _cells;

    public void FindCells()
    {
        _cells = gameObject.GetComponentsInChildren<ArcherPlaceCell>();
        _balloon = gameObject.GetComponentInChildren<Balloon>();

        if (_balloon)
            _balloon._tower = this;
    }

    public Vector3 GetCenterOfArcherCells()
    {
        var result = _cells.Aggregate(Vector3.zero, (current, cell) => current + cell.Position);

        result /= _cells.Length;

        return result;
    }

    public void Reload()
    {
        if(_key)
            _key.Enable();
        
        if(_balloon)
            _balloon.Reload();
    }

    public void Destroy() => Destroy(gameObject);
}
