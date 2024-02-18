using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoisonTowerEffect : TowerEffect
{
    public override void OnStartAim(IEnumerable<Archer> archers)
    {
        foreach (var archer in archers.Take(archers.Count() / 2))
        {
            archer.Kill();
        }
    }

    public override void Setup(Archer archer)
    {
        archer.SetColor(new Color(0, 0.5f, 0));
    }
}