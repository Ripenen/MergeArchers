using System;
using UnityEngine;

public interface IWithId
{
    public int Id { get; }
}

public class SkinsView : BaseShopView<SkinButton, PlayerTowerTemplate>
{
    private PlayerTower _existTower;
    private Player _player;

    public StateMachine MergeMachine;
    private GameFactory _gameFactory;
    private Transform _position;

    public void Setup(PlayerTower existTower, GameFactory gameFactory, Transform position, Player player)
    {
        _position = position;
        _gameFactory = gameFactory;
        _player = player;
        _existTower = existTower;
    }

    protected override Func<PlayerTowerTemplate, bool> IsOpenedCheck => _player.IsOpened;
    protected override int SelectedElementId => _player.Selected;
    protected override int OpenedElementsCount => _player._opened.Count;
    protected override string BuyOneKey => "OpenSkin";
    protected override string GetFromAdKey => "OpenSkin";
    protected override string BuyAllKey => "OpenAllSkins";
    protected override string EventBuyKey => "buySkin2Yn";
    protected override string EventAdKey => "buySkinAd";

    protected override void Open(PlayerTowerTemplate element)
    {
        _player.Open(element);
    }

    protected override void Select(PlayerTowerTemplate element)
    {
        var tower = _gameFactory.CreatePlayerTower(_position, element.Id, _player.TowerRowsCount);
        
        _existTower.CopyArcherPlaceCellsDataTo(tower);
        
        _existTower.Destroy();
        
        _existTower.Rebind(tower, _gameFactory);
        
        MergeMachine.Reload();
        MergeMachine.Run();

        _player.Selected = element.Id;
    }
}