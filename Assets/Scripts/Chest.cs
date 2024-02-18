using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Transform _openPair;
    [SerializeField] private ParticleSystem _golden;
    
    private IPrize _prize;

    public bool Opened { get; private set; }

    public void SetPrize(IPrize prize)
    {
        _prize = prize;
    }

    public void Play() => _golden.Play();

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Opened)
            return;

        _openPair.DOLocalRotate(new Vector3(-80, 0, 0), 1);
        
        _prize.Apply(this);

        Opened = true;
    }
}