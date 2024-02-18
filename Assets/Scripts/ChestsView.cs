using System.Collections;
using DG.Tweening;
using Eiko.YaSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestsView : UiView, IPrizePresenter
{
    [SerializeField] private KeysView _keysView;
    [SerializeField] private Image _prize;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private TextMeshProUGUI _coinsPrize;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _adKeys;
    [SerializeField] private ParticleSystem _mainPrize;

    private ChestsTower _chests;
    private Camera _camera;
    private Player _player;

    public IEnumerator Exit => new ButtonYield(_exitButton);

    public void Setup(Player player, GameFactory factory, Camera camera, ChestsTower chestsTower)
    {
        _chests = chestsTower;
        _camera = camera;
        _player = player;
        
        _keysView.EnableKeys(3);
        
        var prize = factory.GetRandomNotOpenedPlayerTowerTemplate(player);

        if (prize)
        {
            _chests.RandomizePrizes(prize, this);

            _prize.sprite = prize.Icon;
            _coins.text = player.Coins.ToString();
        }
        else
        {
            _prize.gameObject.SetActive(false);
            _chests.RandomizeCoins(this);
        }

        camera.transform.SetPositionAndRotation(_chests.CameraPosition.position, _chests.CameraPosition.rotation);
        
        _exitButton.gameObject.SetActive(false);
        _adKeys.gameObject.SetActive(false);
        
        AdHandler.AddHandler(AddKeys, "AddKeysInChests");
        
        _adKeys.onClick.AddListener(() => AdHandler.TryShowRewardAd("AddKeysInChests", player));
    }

    private void AddKeys()
    {
        _exitButton.gameObject.SetActive(false);
        _adKeys.gameObject.SetActive(false);
        
        _keysView.EnableKeys(2);

        _chests.Activate();
    }

    public void PresentMoney(int count, Chest chest)
    {
        ChangeKeys();

        var cameraPosition = _camera.transform.position;
        var ray = new Ray(cameraPosition, chest.transform.position - cameraPosition);

        _coinsPrize.transform.position = ray.GetPoint(1);
        _coinsPrize.text = "+" + count;
        
        _coinsPrize.transform.localScale = Vector3.zero;

        _coinsPrize.transform.DOScale(Vector3.one, 0.5f);

        _player.Coins += count;
        _coins.text = _player.Coins.ToString();
        
        chest.Play();
    }

    public void PresentTower(PlayerTowerTemplate template, Chest chest)
    {
        ChangeKeys();
        
        _player.Open(template);
        
        var cameraPosition = _camera.transform.position;
        var position = chest.transform.position;
        
        var ray = new Ray(cameraPosition, position - cameraPosition);

        _coinsPrize.transform.position = ray.GetPoint(1);
        
        _coinsPrize.text = YandexSDK.instance.Lang == "ru" ? "Главный приз!" : "Grand Prize!";
        
        _coinsPrize.transform.localScale = Vector3.zero;

        _coinsPrize.transform.DOScale(Vector3.one, 0.5f);

        var prizeEffect = Instantiate(_mainPrize, position, Quaternion.identity);
        
        prizeEffect.Play();
        
        Object.Destroy(prizeEffect.gameObject, prizeEffect.main.duration);
    }

    private void ChangeKeys()
    {
        _keysView.EnableKeys(_keysView.EnabledKeys - 1);

        if (_keysView.EnabledKeys == 0)
        {
            _chests.Deactivate();
            
            _exitButton.gameObject.SetActive(true);
            
            if(_chests.ClosedChests >= 2)
                _adKeys.gameObject.SetActive(true);
        }
    }
}