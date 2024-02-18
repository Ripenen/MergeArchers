using System.Collections.Generic;

public abstract class TowerEffect
{
    public abstract void OnStartAim(IEnumerable<Archer> archer);
    public abstract void Setup(Archer archer);
}