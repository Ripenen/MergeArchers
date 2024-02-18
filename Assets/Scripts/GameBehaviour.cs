using System.Collections;
using System.Collections.Generic;
using RayFire;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameBehaviour
{
    private readonly GameFactory _factory;
    private readonly UiFactory _uiFactory;
    private readonly Camera _camera;
    private readonly Player _player;
    private readonly MagicContainer _magicContainer;

    private StateMachine _stateMachine;
    private GameField _gameFieldInstance;
    private GameUI _gameUI;

    public GameBehaviour(GameFactory factory, UiFactory uiFactory, Camera camera, Player player, MagicContainer magicContainer)
    {
        _factory = factory;
        _uiFactory = uiFactory;
        _camera = camera;
        _player = player;
        _magicContainer = magicContainer;
    }

    public void Run(int levelId)
    {
        _gameFieldInstance = CreateGameField(levelId);

        _gameUI = CreateGameUi(levelId, _gameFieldInstance.PlayerTower.Center, _gameFieldInstance.PlayerTower, _gameFieldInstance.LevelTower);

        _stateMachine = CreateStateMachine(_gameFieldInstance, _gameUI);
        
        _uiFactory.CreateTutorial(_stateMachine).Play();
        
        if(levelId == 1)
            _gameUI.InGameView.HideReload();
    }

    private GameUI CreateGameUi(int levelId, Vector3 trajectoryStart, PlayerTower tower, Tower target)
    {
        _uiFactory.CreateCanvas(_camera);
        
        var inGameView = _uiFactory.CreateInGameView(levelId, Reload, () => MonoCached.StartRoutine(LoadNextLevel()), tower, _player, _magicContainer, _gameFieldInstance, _factory, this);
        var playerAimView = _uiFactory.CreatePlayerAimView(trajectoryStart, levelId);

        playerAimView.Tower = target;

        return new GameUI(inGameView, playerAimView);
    }

    private StateMachine CreateStateMachine(GameField gameFieldInstance, GameUI ui)
    {
        var playerTower = gameFieldInstance.PlayerTower;
        var enemyTower = gameFieldInstance.LevelTower;

        var playerAim = new PlayerAimState(playerTower, ui.PlayerAimView, _camera, ui.InGameView);
        var enemyAim = new EnemyArchersAimState(enemyTower, 1, ui.InGameView);
        
        var shoot = new ShootState(playerTower, _camera, playerAim, enemyTower, ui.InGameView.KeysView);
        var enemyShoot = new ShootState(enemyTower, _camera, enemyAim, playerTower, ui.InGameView.KeysView);
        
        var stateMachine = new StateMachine(new State[]
        {
            new MoveToPlayerAimState(playerTower, _camera.transform, 0),
            playerAim,
            shoot,
            new CheckArchersAliveState(enemyTower, () => MonoCached.StartRoutine(Win())),
            enemyAim,
            enemyShoot,
            new CheckArchersAliveState(playerTower, Lose),
            new MoveToPlayerAimState(playerTower, _camera.transform, 2)
        });

        return stateMachine;
    }

    private void Reload()
    {
        _gameFieldInstance.PlayerTower.TowerEffect = null;
        _gameFieldInstance.LevelTower.TowerEffect = null;
        
        _gameFieldInstance.Reload(_factory);
        _stateMachine.Reload();
        
        _gameUI.InGameView.HideReload();

        _uiFactory.CreateMergeView(_gameFieldInstance.PlayerTower, _stateMachine, _camera, _factory, _gameUI, _gameFieldInstance.Field.PlayerTowerPosition, _player, false);
    }

    private GameField CreateGameField(int levelId)
    {
        GameFieldTemplate gameField = _factory.CreateGameField(levelId);
        PlayerTower playerTower = CreatePlayerTower(gameField.PlayerTowerPosition);
        LevelTower levelTower = CreateLevelTower(gameField.LevelTowerPosition, levelId);
        
        _camera.transform.SetPositionAndRotation(playerTower.AimCameraPosition.position, 
            playerTower.AimCameraPosition.rotation);

        return new GameField(gameField, playerTower, levelTower);
    }

    private PlayerTower CreatePlayerTower(Transform position)
    {
        PlayerTowerTemplate playerTower = _factory.CreatePlayerTower(position, _player.Selected, _player.TowerRowsCount, _player._towerCells);
        List<Archer> playerArchers = _factory.CreateArchersForPlayer(playerTower);

        return new PlayerTower(playerTower, playerArchers);
    }

    private LevelTower CreateLevelTower(Transform position, int level)
    {
        TowerTemplate levelTower = _factory.CreateLevelTower(level, position);
        List<Archer> enemyArchers = _factory.CreateArchersForTower(levelTower, level);

        return new LevelTower(levelTower, enemyArchers, level);
    }

    private IEnumerator LoadNextLevel()
    {
        int newLevelID =_gameFieldInstance.LevelTower.LevelID;

        newLevelID++;

        if (_factory.GameFieldEquals(_gameFieldInstance, newLevelID))
        {
            _gameFieldInstance.PlayerTower.Reload(_factory);

            _gameFieldInstance.LevelTower.Destroy();
            var tower = CreateLevelTower(_gameFieldInstance.Field.LevelTowerPosition, newLevelID);


            _gameFieldInstance = new GameField(_gameFieldInstance, tower);
        }
        else
        {
            var player = _gameFieldInstance.PlayerTower;
            
            _gameFieldInstance.Destroy();

            var field = _factory.CreateGameField(newLevelID);

            player.SetPosition(field.PlayerTowerPosition);
            player.Reload(_factory);

            var tower = CreateLevelTower(field.LevelTowerPosition, newLevelID);

            _gameFieldInstance = new GameField(field, player, tower);
        }
        
        if(RayfireMan.inst)
            Object.Destroy(RayfireMan.inst.gameObject);

        _gameUI.InGameView.HideReload();
        _gameUI.PlayerAimView.Tower = _gameFieldInstance.LevelTower;

        _stateMachine.Stop();

        if (_gameUI.InGameView.KeysView.EnabledKeys == 3)
        {
            ChestsTower chest = _factory.CreateChestTower(new Vector3(1000, 0, 1000));
            ChestsView ui = _uiFactory.CreateChestView(chest, _player, _camera, _factory);

            _player.KeysCount = 0;
            _gameUI.InGameView.Disable();

            yield return ui.Exit;
            
            _gameUI.InGameView.Enable();
            _gameUI.InGameView.KeysView.EnableKeys(0);
            
            Object.Destroy(chest.gameObject);
            Object.Destroy(ui.gameObject);
        }

        _player.KeysCount = _gameUI.InGameView.KeysView.EnabledKeys;

        _gameUI.InGameView.ChangeLevel(newLevelID, _gameFieldInstance.LevelTower);
        _stateMachine = CreateStateMachine(_gameFieldInstance, _gameUI);

        if (_player.ArchesBuyPossibilityCount == 0)
            _player.ArchesBuyPossibilityCount += 1;

        _player.ArchesBuyPossibilityCount += 2;

        _player.Coins += 80 + _player.Level * 20;

        _player.Level += 1;
        
        _uiFactory.CreateMergeView(_gameFieldInstance.PlayerTower, _stateMachine, _camera, _factory, _gameUI, _gameFieldInstance.Field.PlayerTowerPosition, _player, newLevelID == 2);

        _gameUI.PlayerAimView.SetActiveTutorialTrajectory(false);
        _gameUI.PlayerAimView.ResetPreviousTrajectory();

        SaverLoader.Save(_player, _gameFieldInstance.PlayerTower);
    }

    public IEnumerator Win()
    {
        var confetti = _factory.CreateConfetti(_gameFieldInstance.LevelTower.Position + Vector3.right * 5);
        
        Object.Destroy(confetti.gameObject, confetti.main.duration);
        
        _gameUI.InGameView.HideReload();
        
        _gameFieldInstance.PlayerTower.TowerEffect = null;
        _gameFieldInstance.LevelTower.TowerEffect = null;

        yield return new WaitForSeconds(confetti.main.duration);
        
        var winView = _uiFactory.CreateWinView(() => MonoCached.StartRoutine(LoadNextLevel()), _player);

        _player.SkinProgress += 0.1;

        if (_player.SkinProgress >= 1)
        {
            var random = _factory.GetRandomNotOpenedPlayerTowerTemplate(_player);
            
            _player.SkinProgress = 0;

            if(random is null)
                yield break;
            
            _player.Open(random);

            winView.PresentSkin(random.Icon);
        }

        winView.SetProgress(_player.SkinProgress);
        
    }

    private void Lose()
    {
        Sound.Play(Sound.Sounds.Lose);
        
        _uiFactory.CreateLoseView(Reload);
        
        _gameUI.InGameView.KeysView.EnableKeys(_player.KeysCount);

        _gameFieldInstance.PlayerTower.TowerEffect = null;
        _gameFieldInstance.LevelTower.TowerEffect = null;
    }
}