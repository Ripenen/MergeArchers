using System;
using UnityEngine;

public class ArcherPlaceCell : MonoBehaviour
{
    [SerializeField] private bool _hasArcher;
    [SerializeField] private uint _archerLevel = 1;

    public event Action<ArcherPlaceCell> MouseDownOnArcher;
    
    public Vector3 Position => transform.position;

    public bool HasArcher => _hasArcher;
    
    public Vector2 ScreenPosition { get; private set; }
    
    public Archer PlacedArcher { get; private set; }

    public int ArcherLevel => (int)_archerLevel;

    public void CalculateScreenPosition(Camera camera)
    {
        ScreenPosition = camera.WorldToScreenPoint(transform.position);
    }

    public void UpLevel()
    {
        _archerLevel++;
    }

    public void MakeBusy(Archer archer)
    {
        _hasArcher = true;
        _archerLevel = (uint)archer.Level;
        PlacedArcher = archer;

        if(PlacedArcher)
            PlacedArcher.MouseDown -= OnArcherMouseDown;
        
        archer.MouseDown += OnArcherMouseDown;
    }

    public void MakeBusy(int level)
    {
        _hasArcher = true;
        _archerLevel = (uint)level;
    }

    private void OnArcherMouseDown()
    {
        MouseDownOnArcher?.Invoke(this);
    }

    public void MakeFree()
    {
        _hasArcher = false;
        _archerLevel = 1;

        if (PlacedArcher)
            PlacedArcher.MouseDown -= OnArcherMouseDown;

        PlacedArcher = null;
    }

    public void Apply(ArcherPlaceCell fromCell)
    {
        _archerLevel = fromCell._archerLevel;
        _hasArcher = fromCell._hasArcher;
    }
}