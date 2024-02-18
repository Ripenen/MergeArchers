public class TowerPrize : IPrize
{
    private readonly PlayerTowerTemplate _prize;
    private readonly IPrizePresenter _presenter;

    public TowerPrize(PlayerTowerTemplate prize, IPrizePresenter presenter)
    {
        _prize = prize;
        _presenter = presenter;
    }
    
    public void Apply(Chest chest)
    {
        _presenter.PresentTower(_prize, chest);
    }
}