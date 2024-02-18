using System;
using DG.Tweening;
using Eiko.YaSDK;
using UnityEngine;
using UnityEngine.UI;

public class LoseView : UiView
{
    [SerializeField] private Image _image;
    
    public void Setup(Action onFaded)
    {
        _image.DOFade(1, 2).OnComplete(() =>
        {
            YandexSDK.instance.ShowInterstitial();
            onFaded?.Invoke();
            Destroy(gameObject);
        });
    }
}