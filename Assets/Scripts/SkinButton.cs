using System;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public abstract class ShopButton<T, TChild> : MonoBehaviour where TChild : ShopButton<T, TChild>
{
    [SerializeField] private T _skin;
    [SerializeField] private Image _image;
    [SerializeField] private Image _ad;
    [SerializeField] private Button _buy;

    public event Action<TChild, T> Clicked;
    public event Action<TChild, T> BuyClicked;

    public T Skin => _skin;

    private Sprite _sprite;
    private Image _imageComponent;

    public void Init()
    {
        GetComponent<Button>().onClick.AddListener(() => Clicked?.Invoke(this as TChild, _skin));
        _buy.onClick.AddListener(() => BuyClicked?.Invoke(this as TChild, _skin));
        _sprite = _image.sprite;
        _imageComponent = GetComponent<Image>();
    }

    public void Select() => transform.localScale = Vector3.one * 1.25f;
    public void Deselect() => transform.localScale = Vector3.one;
    
    public void Close()
    {
        _imageComponent.color = Color.gray;
        _image.color = Color.gray;
        
        _ad.gameObject.SetActive(true);
        _ad.color = Color.white;
        _buy.gameObject.SetActive(true);
    }
    
    public void Open()
    {
        _ad.gameObject.SetActive(false);
        
        _imageComponent.color = Color.white;
        _image.color = Color.white;
        _buy.gameObject.SetActive(false);
    }
}

public class SkinButton : ShopButton<PlayerTowerTemplate, SkinButton>
{
}