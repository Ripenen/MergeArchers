using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image _bar;

    private AsyncOperation _operation;
    private float _time = 1f;

    void Start() 
    {
        _operation = SceneManager.LoadSceneAsync("Game");

        _operation.allowSceneActivation = false;
    }
    void Update()
    {
        _time -= Time.deltaTime;
        
        if (_time <= 0)
            _operation.allowSceneActivation = true;
        
        SetBarAmount(Mathf.Clamp01((_operation.progress - _time) / 0.9f));
    }

    public void SetBarAmount(float value)
    {
        _bar.fillAmount = value;
    }
}