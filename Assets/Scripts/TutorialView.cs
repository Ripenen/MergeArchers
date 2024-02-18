using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : UiView
{
    [SerializeField] private float _tapReactionDisableTime;
    [SerializeField] private RectTransform _tapReaction;
    [SerializeField] private float _reloadTime;

    [SerializeField] private float _arrowsFadeTime;
    [SerializeField] private float _arrowDelay;
    [SerializeField] private Image _verticalArrow;
    [SerializeField] private Image _horizontalArrow;
    
    private StateMachine _stateMachine;
    private Sequence _tutorial;

    public void Setup(StateMachine notRunStateMachine)
    {
        _stateMachine = notRunStateMachine;
    }
    
    public void Play()
    {
        _tapReaction.gameObject.SetActive(true);
        
        var tutorial = DOTween.Sequence();

        _tutorial?.Kill();
        _tutorial = tutorial;
        
        tutorial.SetDelay(_arrowDelay);

        tutorial.Append(_horizontalArrow.DOFillAmount(1, _arrowsFadeTime));
        tutorial.Append(_horizontalArrow.DOFillAmount(0, _arrowsFadeTime));
        tutorial.Append(_horizontalArrow.DOFillAmount(1, _arrowsFadeTime));
        tutorial.Append(_horizontalArrow.DOFillAmount(0, 0));
        
        tutorial.Append(_verticalArrow.DOFillAmount(1, _arrowsFadeTime * 0.75f));
        tutorial.Append(_verticalArrow.DOFillAmount(0, _arrowsFadeTime * 0.75f));
        tutorial.Append(_verticalArrow.DOFillAmount(1, _arrowsFadeTime));
        tutorial.Append(_verticalArrow.DOFillAmount(0, 0));
        
        tutorial.Play();
        
        StartCoroutine(Timer(_reloadTime, Play));
        StartCoroutine(Timer(_tapReactionDisableTime, () => _tapReaction.gameObject.SetActive(false)));
    }

    private static IEnumerator Timer(float time, Action action)
    {
        yield return new WaitForSeconds(time);
            
        action?.Invoke();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _stateMachine.Run();

            _tutorial?.Kill();

            Destroy();
        }
    }
}