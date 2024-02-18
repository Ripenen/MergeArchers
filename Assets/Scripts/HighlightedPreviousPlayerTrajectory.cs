using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedPreviousPlayerTrajectory : MonoBehaviour
{
    [SerializeField] private Image[] _images;
    [SerializeField] private Color _unhighlightColor;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private float _highlightDuration;

    private bool _highlighted;

    public void Highlight()
    {
        FadeColor(true, _highlightColor);
    }

    public void Unhighlight()
    { 
        FadeColor(false, _unhighlightColor);
    }

    private void FadeColor(bool status, Color fadeColor)
    {
        if (_highlighted == status)
            return;
        
        foreach (var image in _images)
        {
            image.DOKill();
            image.DOColor(fadeColor, _highlightDuration);
        }

        _highlighted = status;
    }
}