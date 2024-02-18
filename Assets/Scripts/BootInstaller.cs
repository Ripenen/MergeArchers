using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Create BootInstaller", fileName = "BootInstaller", order = 0)]
public class BootInstaller : ScriptableObjectInstaller
{
    [SerializeField] private GamePrefabs _prefabs;
    [SerializeField] private ArchersPrefabs _archerPrefabs;
    [SerializeField] private UiPrefabs _uiPrefabs;
    [SerializeField] private MagicContainer _magicContainer;
    [SerializeField] private LevelsStructure _levels;
    [SerializeField] private Sounds _sounds;
    [SerializeField] private int _startLevel;

    public override void InstallBindings()
    {
        Container.Bind<UiPrefabs>().FromInstance(_uiPrefabs).AsSingle();
        Container.Bind<GamePrefabs>().FromInstance(_prefabs).AsSingle();
        Container.Bind<Sounds>().FromInstance(_sounds).AsSingle();
        Container.Bind<int>().FromInstance(_startLevel).AsSingle();
        Container.Bind<MagicContainer>().FromInstance(_magicContainer).AsSingle();

        Container.Bind<GameFactory>().FromInstance(new GameFactory(_prefabs, _archerPrefabs, _levels)).AsSingle();
        Container.Bind<UiFactory>().AsSingle();
        Container.Bind<IInitializable>().To<Boot>().AsSingle();
    }
}

