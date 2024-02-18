using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Create Reference Levels", fileName = "Reference Levels", order = 0)]
public class ReferenceLevels : ScriptableArray<AssetReference>
{
    [SerializeField] private AssetReference[] _levelsPrefabs;
    [SerializeField] private GameFieldTemplate _gameFieldTemplate;
    [SerializeField] private ArchersPrefabs _archers;

    public int _startLevel;
    
    protected override AssetReference[] Objects => _levelsPrefabs;

    public GameFieldTemplate FieldTemplate => _gameFieldTemplate;

    public ArchersPrefabs Archers => _archers;
}