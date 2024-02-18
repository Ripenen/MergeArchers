using UnityEngine;

[CreateAssetMenu(menuName = "Create Levels", fileName = "Levels", order = 0)]
public class Levels : ScriptableArray<TowerTemplate>
{
    [SerializeField] private TowerTemplate[] _levelsPrefabs;
    [SerializeField] private GameFieldTemplate _gameFieldTemplate;
    [SerializeField] private ArchersPrefabs _archers;
    public int _startLevel;

    protected override TowerTemplate[] Objects => _levelsPrefabs;

    public GameFieldTemplate FieldTemplate => _gameFieldTemplate;

    public ArchersPrefabs Archers => _archers;
}