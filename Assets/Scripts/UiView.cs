using UnityEngine;
using UnityEngine.UI;

public abstract class UiView : MonoBehaviour
{
    private void Start()
    {
        var objects = gameObject.GetComponentsInChildren<Button>(true);

        foreach (var button in objects)
        {
            button.onClick.RemoveListener(ClickSoundPlay);
            button.onClick.AddListener(ClickSoundPlay);
        }
    }

    private void ClickSoundPlay()
    {
        Sound.Play(Sound.Sounds.Click);
    }

    public void Enable() => gameObject.SetActive(true);
    public void Disable() => gameObject.SetActive(false);
    public void Destroy() => Destroy(gameObject);

    public bool Active => gameObject.activeSelf;
}