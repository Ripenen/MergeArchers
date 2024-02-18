using System;
using System.Collections.Generic;

public class LevelTower : Tower
{
    public readonly int LevelID;
    
    public LevelTower(TowerTemplate tower, IEnumerable<Archer> instantiatedArchers, int levelID) : base(tower, instantiatedArchers, ArcherType.Enemy)
    {
        LevelID = levelID;
    }

    protected override Func<GameFactory, IEnumerable<Archer>> ReloadMethod => ReloadFunc;

    private IEnumerable<Archer> ReloadFunc(GameFactory factory)
    {
        _tower.Reload();

        return factory.CreateArchersForTower(_tower, LevelID);
    }
}