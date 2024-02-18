using System;

public class MagicShop : BaseShopView<MagicButton, Magic>
{
    private Player _player;
    private InGameView _view;
    protected override Func<Magic, bool> IsOpenedCheck => x => _player._openedMagic.Contains(x.Id);
    protected override int SelectedElementId => _player.SelectedMagic;
    protected override int OpenedElementsCount => _player._openedMagic.Count;
    protected override string BuyOneKey => "OpenMagic";
    protected override string GetFromAdKey => "OpenMagic";
    protected override string BuyAllKey => "OpenAllMagic";
    protected override string EventBuyKey => "buyMagic2Yn";
    protected override string EventAdKey => "buyMagicAd";

    public void Setup(Player player, InGameView view)
    {
        _view = view;
        _player = player;
    }
    
    protected override void Open(Magic element)
    {
        _player.Open(element);
    }

    protected override void Select(Magic element)
    {
        _player.SelectedMagic = element.Id;
        
        _view.ChangeMagicIcon(element.Icon);
    }
}