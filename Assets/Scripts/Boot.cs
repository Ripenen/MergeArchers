using System.Collections;
using Eiko.YaSDK;
using Eiko.YaSDK.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

public class Boot : IInitializable
{
    private readonly GameFactory _factory;
    private readonly UiFactory _uiFactory;
    private readonly int _level;
    private readonly GamePrefabs _prefabs;
    private readonly Sounds _sounds;
    private readonly MagicContainer _magicContainer;

    private const string GameSceneName = "Game";
    private const string LoadSceneName = "Loading";

    public Boot(GameFactory factory, UiFactory uiFactory, int level, GamePrefabs prefabs, Sounds sounds, MagicContainer magicContainer)
    {
        _factory = factory;
        _uiFactory = uiFactory;
        _level = level;
        _prefabs = prefabs;
        _sounds = sounds;
        _magicContainer = magicContainer;
    }

    public void Initialize()
    {
        Sound.Sounds = _sounds;
        
        Object.DontDestroyOnLoad(new GameObject(nameof(MonoCached), typeof(MonoCached)));

        var d = MonoCached.StartRoutine(YandexPrefs.Init());

        SceneManager.sceneLoaded += (arg0, _) =>
        {
            MonoCached.StartRoutine(OnSceneLoaded(arg0, d));
        };
        
        Application.targetFrameRate = 60;
        
        SceneManager.LoadScene(LoadSceneName);
    }

    private IEnumerator OnSceneLoaded(Scene scene, Coroutine d)
    {
        if (scene.name == LoadSceneName)
        {
            yield break;
        }
        
        //SaverLoader.Delete();

        yield return d;
        
        var player = SaverLoader.LoadOrCreate(_prefabs.PlayerTowers[0], _level);
        
        if (player.NoAd)
        {
            YandexSDK.Instance.AdsOff();
        }
        
        Sound.Play(_sounds.Background, true);

        //player.Level = 2;
        //player.SelectedMagic = 3;
        
        new GameBehaviour(_factory, _uiFactory, Camera.main, player, _magicContainer).Run(player.Level);

        var process = new PurchaseProcess();
        
        process.InitPurchases();
        
        AdHandler.Init();
    }
}