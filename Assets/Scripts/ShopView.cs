using Eiko.YaSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : UiView
{
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _skipAd;
    
    [SerializeField] private Button _buyAdNo;
    [SerializeField] private Button _close;
    
    [SerializeField] private Button _buyCoins;
    [SerializeField] private Button _buyCoins2X;
    
    [SerializeField] private Button _buySkipAd;
    [SerializeField] private Button _buySkipAd2X;

    private PlayerTower _playerTower;
    private MergeView _mergeView;

    public void Setup(Player player, PlayerTower playerTower, MergeView mergeView)
    {
        _mergeView = mergeView;
        _coins.text = player.Coins.ToString();
        
        _skipAd.text = player.SkipAdCount.ToString();
        
        _close.onClick.AddListener(Disable);
        
        if(player.NoAd)
            Destroy(_buyAdNo.gameObject);
        
        _buyCoins.onClick.AddListener(() => PurchaseProcess.instance.ProcessPurchase("buy50kCoins", () => BuyCoins(player, 600)));
        _buyCoins2X.onClick.AddListener(() => PurchaseProcess.instance.ProcessPurchase("buy100kCoins", () => BuyCoins(player, 50000)));
        
        _buySkipAd.onClick.AddListener(() => PurchaseProcess.instance.ProcessPurchase("buy10SkipAd", () => BuySkipAd(player, 10)));
        _buySkipAd2X.onClick.AddListener(() => PurchaseProcess.instance.ProcessPurchase("buy30SkipAd", () => BuySkipAd(player, 30)));
        
        _buyAdNo.onClick.AddListener(() => PurchaseProcess.instance.ProcessPurchase("buyNoAd", () => DisableAd(player)));

        _playerTower = playerTower;
    }

    private void DisableAd(Player player)
    {
        YandexSDK.instance.AdsOff();
        player.NoAd = true;
        Destroy(_buyAdNo.gameObject);
        
        SaverLoader.Save(player, _playerTower);
    }

    private void BuySkipAd(Player player, int count)
    {
        player.SkipAdCount += count;
        
        _skipAd.text = player.SkipAdCount.ToString();
        
        SaverLoader.Save(player, _playerTower);
    }
    
    private void BuyCoins(Player player, int count)
    {
        player.Coins += count;
        _coins.text = player.Coins.ToString();

        _mergeView.UpdateCoins(player);
        
        SaverLoader.Save(player, _playerTower);
    }
}