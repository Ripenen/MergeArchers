using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Tower
{
    protected TowerTemplate _tower;
    private readonly ArcherType _type;

    public TowerEffect TowerEffect;
    
    private IEnumerable<Archer> _instantiatedArchers;

    protected Tower(TowerTemplate tower, IEnumerable<Archer> instantiatedArchers, ArcherType type)
    {
        _tower = tower;
        _instantiatedArchers = instantiatedArchers;
        _type = type;
    }

    public IEnumerable<Archer> AliveArchers => _instantiatedArchers.Where(x => x && !x.Killed);
    public Vector3 Center => _tower.GetCenterOfArcherCells();
    public Vector3 Position => _tower.transform.position;

    public bool AwaitShoot => _tower._balloon && _tower._balloon.Dyeing;

    protected abstract Func<GameFactory, IEnumerable<Archer>> ReloadMethod { get; }

    public void Reload(GameFactory factory)
    {
        foreach (var archer in _instantiatedArchers)
            archer.Destroy();
        
        _instantiatedArchers = ReloadMethod(factory);
    }
    
    public void MakeParentOf(MonoBehaviour archer)
    {
        archer.transform.parent = _tower.transform;
    }

    protected void AddArcher(Archer archer)
    {
        _instantiatedArchers = _instantiatedArchers.Concat(new[] {archer});
    }

    public void Destroy()
    {
        _tower.Destroy();
    }
}
