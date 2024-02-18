using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTower : Tower
{
    private new PlayerTowerTemplate _tower;

    public bool IsBow { get; private set; } = true;

    public PlayerTower(PlayerTowerTemplate tower, IEnumerable<Archer> instantiatedArchers) : base(tower, instantiatedArchers, ArcherType.Player)
    {
        _tower = tower;
    }
    
    public Transform AimCameraPosition => _tower.AimCameraPosition;

    public Transform MergeCameraPosition => _tower.MergeCameraPosition;

    public IEnumerable<ArcherPlaceCell> Cells => _tower.Cells;
    public List<ArcherCell> SerializableCells => _tower.Cells.Select(x => new ArcherCell(x.HasArcher, x.ArcherLevel)).ToList();

    protected override Func<GameFactory, IEnumerable<Archer>> ReloadMethod =>
        factory => factory.CreateArchersForPlayer(_tower); 

    public new void AddArcher(Archer archer)
    {
        base.AddArcher(archer);
    }

    public void ChangeRowsCount(int rows)
    {
        _tower.ActivateRows(rows);
    }

    public void ActiveBombGun()
    {
        Sound.Play(Sound.Sounds.BombSelect);

        IsBow = false;
        
        foreach (var aliveArcher in AliveArchers)
        {
            aliveArcher.ActivateGun();
        }
    }
    
    public void DeactivateBombGun()
    {
        IsBow = true;
        
        foreach (var aliveArcher in AliveArchers)
        {
            aliveArcher.ActivateBow();
        }
    }

    public void SetPosition(Transform fieldPlayerTowerPosition)
    {
        _tower.transform.position = fieldPlayerTowerPosition.position;
    }

    public void CopyArcherPlaceCellsDataTo(PlayerTowerTemplate to)
    {
        _tower.CopyPlaceCellsDataTo(to);
    }

    public void Rebind(PlayerTowerTemplate template, GameFactory factory)
    {
        _tower = template;
        base._tower = template;
        
        Reload(factory);
    }
}