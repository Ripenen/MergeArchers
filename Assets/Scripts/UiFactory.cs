using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class UiFactory
{
    private readonly UiPrefabs _prefabs;

    private Transform _canvas;

    public UiFactory(UiPrefabs prefabs)
    {
        _prefabs = prefabs;
    }

    public void CreateCanvas(Camera camera)
    {
        var canvas = Object.Instantiate(_prefabs.CanvasTemplate);

        canvas.worldCamera = camera;
        _canvas = canvas.transform;

        canvas.planeDistance = 1;
    }

    public PlayerAimView CreatePlayerAimView(Vector3 trajectoryStartPosition, int level)
    {
        var view = Object.Instantiate(_prefabs.PlayerAimView, _canvas);
        
        view.Setup(trajectoryStartPosition, level);

        view.Disable();
        
        return view;
    }
    
    public MergeView CreateMergeView(PlayerTower playerTower, StateMachine notRunStateMachine, Camera camera, GameFactory factory, GameUI ui, Transform player, Player player1, bool tutorial)
    {
        var view = Object.Instantiate(_prefabs.MergeView, _canvas);

        var skin = Object.Instantiate(_prefabs.SkinsView, view.transform);
        var magic = Object.Instantiate(_prefabs.MagicShop, view.transform);
        var shop = Object.Instantiate(_prefabs.ShopView, view.transform);
        
        skin.Setup(playerTower, factory, player, player1);
        magic.Setup(player1, ui.InGameView);

        skin.Init(player1, playerTower);
        magic.Init(player1, playerTower);
        
        shop.Setup(player1, playerTower, view);

        view.Setup(playerTower, notRunStateMachine, camera, factory, ui, skin, player1, tutorial, this, shop, magic);
        
        skin.Disable();
        magic.Disable();
        shop.Disable();
        
        return view;
    }
    
    public InGameView CreateInGameView(int levelId, Action reload, Action skip, PlayerTower tower, Player player, MagicContainer magicContainer, GameField target, GameFactory factory, GameBehaviour behaviour)
    {
        var view = Object.Instantiate(_prefabs.InGameView, _canvas);
        
        view.Setup(levelId, reload, skip, tower, player, magicContainer, target.LevelTower, factory, behaviour);

        return view;
    }

    public TutorialView CreateTutorial(StateMachine notRunStateMachine)
    {
        var view = Object.Instantiate(_prefabs.TutorialView, _canvas);
        
        view.Setup(notRunStateMachine);

        return view;
    }
    
    public LoseView CreateLoseView(Action faded)
    {
        var view = Object.Instantiate(_prefabs.LoseView, _canvas);
        
        view.Setup(faded);

        return view;
    }
    
    public WinView CreateWinView(Action next, Player player)
    {
        var view = Object.Instantiate(_prefabs.WinView, _canvas);
        
        view.Setup(next, player);

        return view;
    }

    public ChestsView CreateChestView(ChestsTower chest, Player player, Camera camera, GameFactory factory)
    {
        var view = Object.Instantiate(_prefabs.ChestsView, _canvas);
        
        view.Setup(player, factory, camera, chest);

        return view;
    }
}