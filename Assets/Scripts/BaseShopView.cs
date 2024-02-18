using System;
using System.Linq;
using ModestTree;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseShopView<TButton, TShopElement> : UiView where TButton : ShopButton<TShopElement, TButton> where TShopElement : IWithId
{
    [SerializeField] private TButton[] _buttons;
    [SerializeField] private Button _close;
    [SerializeField] private Button _openAll;
    
    private PlayerTower _tower;
    private Player _player1;

    protected abstract Func<TShopElement, bool> IsOpenedCheck { get; }
    protected abstract int SelectedElementId { get; }
    protected abstract int OpenedElementsCount { get; }
    
    protected abstract string BuyOneKey { get; }
    protected abstract string GetFromAdKey { get; }
    protected abstract string BuyAllKey { get; }
    protected abstract string EventBuyKey { get; }
    protected abstract string EventAdKey { get; }

    protected abstract void Open(TShopElement element);
    protected abstract void Select(TShopElement element);

    public void Init(Player player, PlayerTower tower)
    {
        _player1 = player;
        _tower = tower;
        
        foreach (var skinButton in _buttons)
        {
            skinButton.Init();
            
            if (IsOpenedCheck(skinButton.Skin))
                skinButton.Open();
            else
                skinButton.Close();
            
            if(SelectedElementId == skinButton.Skin.Id)
                skinButton.Select();
            
            skinButton.Clicked += OnShopElementClicked;
            skinButton.BuyClicked += BuyShopElement;
        }
        
        if(OpenedElementsCount == _buttons.Length)
            _openAll.gameObject.SetActive(false);

        _close.onClick.AddListener(Disable);
        _openAll.onClick.AddListener(OpenAllShopElements);
    }

    private void OpenAllShopElements()
    {
        PurchaseProcess.instance.ProcessPurchase(BuyAllKey, () =>
        {
            foreach (var skinButton in _buttons.Skip(1))
            {
                Open(skinButton.Skin);
            }
        
            _openAll.gameObject.SetActive(false); 
            
            SaverLoader.Save(_player1, _tower);
        });
    }

    private void BuyShopElement(TButton arg1, TShopElement arg2)
    {
        PurchaseProcess.instance.ProcessPurchase(BuyOneKey + _buttons.IndexOf(arg1), () =>
        {
            Open(arg2);
            arg1.Open();
        
            if(OpenedElementsCount == _buttons.Length)
                _openAll.gameObject.SetActive(false);
            
            AppMetricaWeb.Event(EventBuyKey);

            SaverLoader.Save(_player1, _tower);
        });
    }

    private void OnShopElementClicked(TButton button, TShopElement shopElement)
    {
        if (IsOpenedCheck(shopElement) == false)
        {
            AdHandler.AddHandler(() =>
            {
                Open(shopElement);
                
                button.Open();
            
                if(OpenedElementsCount == _buttons.Length)
                    _openAll.gameObject.SetActive(false);
                
                AppMetricaWeb.Event(EventAdKey);

            }, GetFromAdKey);
            
            AdHandler.TryShowRewardAd(GetFromAdKey, _player1);
        }
        
        Select(shopElement);
        
        foreach (var shopButton in _buttons)
        {
            shopButton.Deselect();
        }
        
        button.Select();
        
        SaverLoader.Save(_player1, _tower);
    }

    private void OnDestroy()
    {
        foreach (var skinButton in _buttons)
        {
            skinButton.Clicked -= OnShopElementClicked;
            skinButton.BuyClicked -= BuyShopElement;
        }
    }
}