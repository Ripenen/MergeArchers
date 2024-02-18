using System;
using System.Linq;
using DG.Tweening;
using Eiko.YaSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class InGameView : UiView
{
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private Button _reload;
    [SerializeField] private Button _skip;
    [SerializeField] private Button _bomb;
    [SerializeField] private Image _bombFill;
    [SerializeField] private Image _bombAd;
    [SerializeField] private Image _hand;
    [SerializeField] private Image _magic;
    [SerializeField] private KeysView _keysView;

    private int _levelID;
    private Player _player;
    private MagicContainer _container;
    private LevelTower _target;
    private GameBehaviour _behaviour;

    public KeysView KeysView => _keysView;

    public void Setup(int levelId, Action onReloadClicked, Action skipClicked, PlayerTower tower, Player player, MagicContainer container, LevelTower target, GameFactory factory, GameBehaviour behaviour)
    {
        _behaviour = behaviour;
        _container = container;
        _player = player;
        
        if(levelId < 10)
            _keysView.Disable();
        
        _keysView.EnableKeys(player.KeysCount);

        _level.text = YandexSDK.instance.Lang == "ru" ? $"Уровень {levelId}" : $"Level {levelId}";
        _levelID = levelId;
        
        _reload.onClick.RemoveAllListeners();
        
        _reload.onClick.AddListener(() =>
        {
            if(_hand.gameObject.activeInHierarchy)
                return;
            
            onReloadClicked?.Invoke();
            
            _keysView.EnableKeys(player.KeysCount);
            
            AppMetricaWeb.Event("resetLvl");
        });
        
        _skip.onClick.RemoveAllListeners();
        
        AdHandler.AddHandler(() =>
        {
            skipClicked?.Invoke();
            
            AppMetricaWeb.Event("skipLvlAd");
        }, "Skip");
        
        _skip.onClick.AddListener(() =>
        {
            if(_hand.gameObject.activeInHierarchy)
                return;
            
            AdHandler.TryShowRewardAd("Skip", player);
        });
        
        SetupMagic(levelId, tower, player, target, factory);
    }

    private void SetupMagic(int levelId, PlayerTower tower, Player player, LevelTower target, GameFactory factory)
    {
        _target = target;

        if (player.SelectedMagic == _container.BombId)
        {
            ChangeMagicIcon(_container.BombIcon);
        }
        else
        {
            var magic = _container.Magics.First(x => x.Id == player.SelectedMagic);

            ChangeMagicIcon(magic.Icon);
        }
        
        if (levelId <= 5)
        {
            _bomb.gameObject.SetActive(false);
        }

        _bomb.interactable = false;
        _hand.gameObject.SetActive(false);
        if (levelId == 6)
        {
            _hand.gameObject.SetActive(true);
            _bombFill.fillAmount = 1;
            _bomb.interactable = true;

            _hand.transform.DOScale(Vector3.one * 1.25f, 1).SetLoops(-1, LoopType.Yoyo);
        }

        _bomb.onClick.RemoveAllListeners();

        AdHandler.AddHandler(() =>
        {
            if (player.SelectedMagic == _container.BombId)
            {
                tower.ActiveBombGun();
            }
            else
            {
                var magic = _container.Magics.First(x => x.Id == player.SelectedMagic);
                
                var magicObject = Instantiate(magic);

                magicObject.Run(tower, _target, Camera.main, factory, _behaviour);
            }

            _hand.gameObject.SetActive(false);

            _bombFill.fillAmount = 0;
            _bomb.interactable = false;
            _bombAd.gameObject.SetActive(false);
        }, "Bomb");

        _bomb.onClick.AddListener(() =>
        {
            AppMetricaWeb.Event("useBomb");
            
            if (_bombAd.gameObject.activeInHierarchy)
            {
                AdHandler.TryShowRewardAd("Bomb", player);
                return;
            }

            if (player.SelectedMagic == _container.BombId)
            {
                tower.ActiveBombGun();
            }
            else
            {
                var magic = _container.Magics.First(x => x.Id == player.SelectedMagic);
                
                var magicObject = Instantiate(magic);

                magicObject.Run(tower, _target, Camera.main, factory, _behaviour);
            }

            _hand.gameObject.SetActive(false);

            if (_bombAd.gameObject.activeInHierarchy)
            {
                _bombFill.fillAmount = 0;
                _bomb.interactable = false;
                _bombAd.gameObject.SetActive(false);
            }
            else
            {
                _bombFill.fillAmount = 1;
                _bombAd.gameObject.SetActive(true);
                _bomb.gameObject.SetActive(false);
            }
        });
    }

    public void HideReload()
    {
        _reload.gameObject.SetActive(false);
        _skip.gameObject.SetActive(false);
        _bomb.gameObject.SetActive(false);
    }

    public void SetActiveKeys(bool value)
    {
        switch (value)
        {
            case true when _levelID >= 10:
                _keysView.Enable();
                break;
            case false:
                _keysView.Disable();
                break;
        }
    }

    public void ChangeMagicIcon(Sprite icon)
    {
        _magic.sprite = icon;
    }

    public void ShowReload()
    {
        _reload.gameObject.SetActive(true);
        _skip.gameObject.SetActive(true);
        
        _bomb.gameObject.SetActive(_levelID > 5);

        if (_bomb.isActiveAndEnabled && _levelID != 6)
        {
            _bomb.interactable = false;
            _bombFill.fillAmount = 0;
            _bombAd.gameObject.SetActive(false);
        }
    }

    public void ShowTut()
    {
        _hand.gameObject.SetActive(true);
        _bombFill.fillAmount = 1;
        _bomb.interactable = true;

        _hand.transform.DOScale(Vector3.one * 1.25f, 1).SetLoops(-1, LoopType.Yoyo);
    }

    public void ChangeLevel(int levelId, LevelTower levelTower)
    {
        _levelID = levelId;

        _target = levelTower;

        if(levelId >= 10)
            _keysView.Enable();

        _level.text = YandexSDK.instance.Lang == "ru" ? $"Уровень {levelId}" : $"Level {levelId}";
        
        _keysView.EnableKeys(_player.KeysCount);
    }

    public void UpBombLoad()
    {
        _bombFill.DOFillAmount(_bombFill.fillAmount + 0.34f, 0.5f).OnComplete(() =>
        {
            if (_bombFill.fillAmount >= 1)
                _bomb.interactable = true;
        });
    }

    public void SetBombActive(bool value)
    {
        _bomb.gameObject.SetActive(value && _levelID > 5);
        _hand.gameObject.SetActive(_bomb.gameObject.activeInHierarchy && _levelID == 6);
    }
}