using System.Linq;
using UnityEngine;

public class ChestsTower : MonoBehaviour
{
    [SerializeField] private Chest[] _chests;
    [SerializeField] private Transform _cameraPosition;

    public Transform CameraPosition => _cameraPosition;
    public int ClosedChests => _chests.Count(x => !x.Opened);

    public void RandomizePrizes(PlayerTowerTemplate mainPrizeTowerTemplate, IPrizePresenter presenter)
    {
        var mainPrize = new TowerPrize(mainPrizeTowerTemplate, presenter);

        var indexMainPrizeChest = Random.Range(0, _chests.Length);
        
        _chests[indexMainPrizeChest].SetPrize(mainPrize);

        for (int i = 0; i < _chests.Length; i++)
        {
            if(i == indexMainPrizeChest)
                continue;
            
            _chests[i].SetPrize(new CoinsPrize(Random.Range(0, 100), presenter));
        }
    }

    public void Deactivate()
    {
        foreach (var chest in _chests)
        {
            chest.enabled = false;
        }
    }

    public void Activate()
    {
        foreach (var chest in _chests)
        {
            chest.enabled = true;
        }
    }

    public void RandomizeCoins(IPrizePresenter prizePresenter)
    {
        foreach (var chest in _chests)
        {
            chest.SetPrize(new CoinsPrize(Random.Range(0, 100), prizePresenter));
        }
    }
}