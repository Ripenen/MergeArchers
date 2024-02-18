using System;
using DG.Tweening;
using Eiko.YaSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


public class WinView : UiView
{
    [SerializeField] private Button _next;
    [SerializeField] private Slider _skinProgression;
    [SerializeField] private Image _skinPresent;
    [SerializeField] private TextMeshProUGUI _skinProgress;
    [SerializeField] private TextMeshProUGUI _reward;

    [SerializeField] private RewardSlider _slider;
    [SerializeField] private Button _rewardButton;
    [SerializeField] private Image _bomb;

    private AudioSource _win;

    public void Setup(Action next, Player player)
    {
        var reward = 100 + player.Level * 20;

        _win = Sound.Play(Sound.Sounds.Win);
        
        _next.onClick.AddListener(() =>
        {
            next?.Invoke();
            
            if(_win)
                Object.Destroy(_win);
            
            YandexSDK.instance.ShowInterstitial();
            
            if(player.Level == 1)
                AppMetricaWeb.Event("nextLvl");

            Destroy(gameObject);
        });
        
        if (player.Level <= 5)
        {
            _bomb.transform.parent.gameObject.SetActive(true);

            _bomb.fillAmount = (player.Level - 1) / 5f;
            _bomb.DOFillAmount(player.Level / 5f, 1);
            
            _slider.gameObject.SetActive(false);
            _rewardButton.gameObject.SetActive(false);
        }
        else
        {
            _bomb.transform.parent.gameObject.SetActive(false);

            _slider.gameObject.SetActive(true);
            _rewardButton.gameObject.SetActive(true);
        }

        if(_slider.isActiveAndEnabled)
            _slider.Setup(reward);
        
        AdHandler.AddHandler(() =>
        {
            player.Coins += _slider.ScaledReward;
            next?.Invoke();
            
            if(_win)
                Object.Destroy(_win);
            
            Destroy(gameObject);
            
            AppMetricaWeb.Event("nextLvlRouletteAd");
            
        }, "RewardWin");
        
        if(_rewardButton.isActiveAndEnabled)
            _rewardButton.onClick.AddListener(() =>
            {
                AdHandler.TryShowRewardAd("RewardWin", player);
            });

        _reward.text = reward.ToString();
    }

    public void SetProgress(double progress)
    {
        _skinProgression.value = (float)progress;
        _skinProgress.text = (int)(progress * 100) + "%";
    }

    public void PresentSkin(Sprite sprite)
    {
        _skinProgression.gameObject.SetActive(false);
        _skinPresent.gameObject.SetActive(true);

        _skinPresent.sprite = sprite;
    }
}