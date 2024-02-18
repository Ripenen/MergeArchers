using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MergeView : UiView
{
    [SerializeField] private Button _startBattle;
    [SerializeField] private Button _buyArcher;
    [SerializeField] private Button _skins;
    [SerializeField] private Button _sizeUp;
    [SerializeField] private Button _shop;
    [SerializeField] private Button _map;
    [SerializeField] private Button _magic;
    [SerializeField] private Transform _mergePointer;
    [SerializeField] private ParticleSystem _mergeEffectTemplate;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _archersPrice;
    [SerializeField] private Image _adArcherBuy;
    [SerializeField] private Image _mergeTutorialHand;
    [SerializeField] private Slider _levelProgressSlider;
    [SerializeField] private TextMeshProUGUI _levelProgressText;

    public void Setup(PlayerTower playerTower, StateMachine notRunStateMachine, Camera camera, GameFactory factory, GameUI ui, SkinsView skins, Player player, bool tutorial, UiFactory uiFactory, ShopView shop, MagicShop magicShop)
    {
        var merge = new MergingState(playerTower, camera, factory, _mergePointer, _mergeEffectTemplate, _mergeTutorialHand.gameObject);
        
        var machine = new StateMachine(new State[]
        {
            new MoveToPlayerMergeState(playerTower, camera.transform),
            merge,
            new MoveToPlayerAimState(playerTower, camera.transform, 1),
            new StopMachineState()
        });

        if (tutorial)
        {
            Scale();
            _mergeTutorialHand.gameObject.SetActive(true);
        }
        else
        {
            _mergeTutorialHand.gameObject.SetActive(false);
        }

        _shop.onClick.AddListener(shop.Enable);

        _startBattle.onClick.AddListener(() => StartBattle(notRunStateMachine, ui, player, uiFactory, machine, camera));

        if (player.ArchesBuyPossibilityCount <= 0)
        {
            Object.Destroy(_buyArcher.gameObject);
        }

        if (player.ArchesBuyPossibilityCount <= 1)
        {
            _adArcherBuy.gameObject.SetActive(true);
            _archersPrice.gameObject.SetActive(false);
        }

        var price = 20 + playerTower.AliveArchers.Sum(aliveArcher => aliveArcher.Level * 5);

        if (player.Coins < price)
        {
            Object.Destroy(_buyArcher.gameObject); 
        }

        _archersPrice.text = price.ToString();

        AdHandler.AddHandler(() =>
        {
            if(factory.AddArcher(playerTower, merge) == false)
                return;
            
            player.ArchesBuyPossibilityCount -= 1;
            
            Object.Destroy(_buyArcher.gameObject);
            
            AppMetricaWeb.Event("buyArcherAd");
        }, "BuyArcher");

        _buyArcher.onClick.AddListener(() =>
        {
            if(_mergeTutorialHand.gameObject.activeInHierarchy && playerTower.AliveArchers.Count() > 1)
                return;
            
            if (player.ArchesBuyPossibilityCount == 1)
            {
                AdHandler.TryShowRewardAd("BuyArcher", player);
                return;
            }

            Sound.Play(Sound.Sounds.CreateUnit);

            if (player.ArchesBuyPossibilityCount > 1)
            {
                if (player.Coins < price)
                {
                    Object.Destroy(_buyArcher.gameObject);
                    
                    return;
                }
                    
                if(factory.AddArcher(playerTower, merge) == false)
                    return;

                player.Coins -= price;
                _coins.text = player.Coins.ToString();
            
                price += 10;
                _archersPrice.text = price.ToString();
                
                if (player.Coins < price)
                {
                    Object.Destroy(_buyArcher.gameObject); 
                }

                player.ArchesBuyPossibilityCount -= 1;

                if (player.ArchesBuyPossibilityCount == 1)
                {
                    _adArcherBuy.gameObject.SetActive(true);
                    _archersPrice.gameObject.SetActive(false);
                }
                
                CC(playerTower, camera);
                
                AppMetricaWeb.Event("buyArcherGold");
                
                return;
            }

            if(factory.AddArcher(playerTower, merge) == false)
                return;

            player.ArchesBuyPossibilityCount -= 1;

            if (player.ArchesBuyPossibilityCount <= 0)
            {
                Object.Destroy(_buyArcher.gameObject);
            }

            if (player.ArchesBuyPossibilityCount == 1)
            {
                _adArcherBuy.gameObject.SetActive(true);
                _archersPrice.gameObject.SetActive(false);
            }
        });

        if(player.TowerRowsCount == 4)
            Destroy(_sizeUp.gameObject);

        AdHandler.AddHandler(() =>
        {
            player.TowerRowsCount++;
            
            if(player.TowerRowsCount == 4)
                Destroy(_sizeUp.gameObject);
            
            playerTower.ChangeRowsCount(player.TowerRowsCount);

            playerTower.Reload(factory);
            
            machine.Reload();
            machine.Run();
            
            AppMetricaWeb.Event("buyTowerSizeAd");
            
            SaverLoader.Save(player, playerTower);
        }, "SizeUp");

        _sizeUp.onClick.AddListener(() =>
        {
            if(_mergeTutorialHand.gameObject.activeInHierarchy)
                return;
            
            AdHandler.TryShowRewardAd("SizeUp", player);
        });

        _coins.text = player.Coins.ToString();

        skins.MergeMachine = machine;
        _skins.onClick.AddListener(() =>
        {
            if(_mergeTutorialHand.gameObject.activeInHierarchy)
                return;
            
            AppMetricaWeb.Event("openSkinShop");
            
            skins.Enable();
        });

        _magic.onClick.AddListener(() =>
        {
            if(_mergeTutorialHand.gameObject.activeInHierarchy)
                return;
            
            magicShop.Enable();
        });
        
        machine.Run();
        
        _map.onClick.AddListener(() => factory.CreateMap(camera, player.Level, this, ui.InGameView, playerTower.MergeCameraPosition));
        
        UpdateLevelProgress(factory, player);
    }

    private void Scale()
    {
        _mergeTutorialHand.transform.DOScale(1.25f, 1).OnComplete(() =>
        {
            _mergeTutorialHand.transform.DOScale(1, 1).OnComplete(Scale);
        });
    }

    private void StartBattle(StateMachine notRunStateMachine, GameUI ui, Player player, UiFactory uiFactory,
        StateMachine machine, Camera camera)
    {
        if (_mergeTutorialHand.gameObject.activeInHierarchy)
            return;

        if (player.Level == 6)
            ui.InGameView.ShowTut();

        Destroy(gameObject);
        
        TryShowHorsesOrBalloons(camera, ui, uiFactory, machine, notRunStateMachine);
    }
    
    private void TryShowHorsesOrBalloons(Camera camera, GameUI ui, UiFactory uiFactory, StateMachine machine,
        StateMachine notRunStateMachine)
    {
        var balloon = FindObjectOfType<Balloon>();

        if (balloon)
        {
            var z = balloon.transform.position.z;

            camera.transform.position = balloon.transform.position - new Vector3(1, 0.25f, 0) * 5;
            
            balloon.transform.position -= Vector3.forward * 3;
            balloon.PlacedArcher.transform.position -= Vector3.forward * 3;
            
            camera.transform.LookAt(balloon.transform.position);

            balloon.PlacedArcher.transform.DOMoveZ(z, 2);
            balloon.transform.DOMoveZ(z, 2)
                .OnUpdate(() => camera.transform.LookAt(balloon.transform.position))
                .OnComplete(() =>
                {
                    ui.InGameView.ShowReload();
                    uiFactory.CreateTutorial(notRunStateMachine).Play();
                    machine.TransitToNext();

                    AppMetricaWeb.Event("startBattle");
                });
            
            return;
        }

        var horses = FindObjectsOfType<Horse>();

        if (horses.Length > 0)
        {
            var result = horses.Aggregate(Vector3.zero, (current, cell) => current + cell.transform.position);

            result /= horses.Length;
            
            camera.transform.position = result - new Vector3(1, -0.75f, 0) * 5;
            camera.transform.LookAt(result);

            Tween tween = null;

            foreach (var horse in horses)
            {
                horse.transform.position += Vector3.right * 2;
                horse.PlacedArcher.transform.position += Vector3.right * 2; 
                
                horse.PlacedArcher.transform.DOMoveX((horse.PlacedArcher.transform.position - Vector3.right * 2).x, 2);
                tween = horse.transform.DOMoveX((horse.transform.position - Vector3.right * 2).x, 2);
            }
            
            tween.OnUpdate(() =>
                {
                    result = horses.Aggregate(Vector3.zero, (current, cell) => current + cell.transform.position);

                    result /= horses.Length;
                    
                    camera.transform.LookAt(result);
                })
                .OnComplete(() =>
                {
                    ui.InGameView.ShowReload();
                    uiFactory.CreateTutorial(notRunStateMachine).Play();
                    machine.TransitToNext();

                    AppMetricaWeb.Event("startBattle");
                });
            return;
        }
        
        ui.InGameView.ShowReload();
        uiFactory.CreateTutorial(notRunStateMachine).Play();
        machine.TransitToNext();

        AppMetricaWeb.Event("startBattle");
    }

    private void UpdateLevelProgress(GameFactory factory, Player player)
    {
        var levels = factory.GetLevelsFromStructure(player.Level);

        var d = player.Level - 1 - levels._startLevel;

        _levelProgressSlider.DOValue(d * 1f / (levels.Length), 1);
        _levelProgressText.text = MathF.Round((d * 1f / (levels.Length)) * 100, MidpointRounding.ToEven) + "%";

        MonoCached.StartRoutine(DisableF());
        
        if(d * 1f / (levels.Length) == 0 && player.Level > 1)
            _map.onClick.Invoke();

        IEnumerator DisableF()
        {
            yield return new WaitForSeconds(2f);

            if (_levelProgressSlider)
                _levelProgressSlider.gameObject.SetActive(false);
        }
    }

    private void CC(PlayerTower playerTower, Camera camera)
    {
        var first = playerTower.AliveArchers.First();
        var second = playerTower.AliveArchers.First(x => x != first);

        var ray = camera.ScreenPointToRay(camera.WorldToScreenPoint(first.transform.position));
        var ray2 = camera.ScreenPointToRay(camera.WorldToScreenPoint(second.transform.position));
        
        
        _mergeTutorialHand.transform.DOKill();
        _mergeTutorialHand.transform.localScale = Vector3.one * 0.75f;
        
        _mergeTutorialHand.transform.position = ray.GetPoint(camera.nearClipPlane) - new Vector3(0.05f, 0.1f);
        
        _mergeTutorialHand.transform.DOMove(ray2.GetPoint(camera.nearClipPlane) - new Vector3(0, 0.1f), 1).SetLoops(-1, LoopType.Yoyo);
    }

    public void UpdateCoins(Player player)
    {
        _coins.text = player.Coins.ToString();
    }
}