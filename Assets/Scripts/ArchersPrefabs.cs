using UnityEngine;

[CreateAssetMenu(menuName = "Create ArchersPrefabs", fileName = "ArchersPrefabs", order = 0)]
public class ArchersPrefabs : ScriptableArray<Archer>
{
    [SerializeField] private Archer[] _archersPrefabsByLevels;

    protected override Archer[] Objects => _archersPrefabsByLevels;
}