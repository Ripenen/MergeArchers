public class CoinsPrize : IPrize
{
    private readonly int _count;
    private readonly IPrizePresenter _presenter;

    public CoinsPrize(int count, IPrizePresenter presenter)
    {
        _count = count;
        _presenter = presenter;
    }

    public void Apply(Chest chest)
    {
        _presenter.PresentMoney(_count, chest);
    }
}