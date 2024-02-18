using UnityEngine;
using UnityEngine.UI;

public class KeysView : UiView
{
    [SerializeField] private Image[] _keys;
    
    public int EnabledKeys { get; private set; }

    public void EnableKeys(int count)
    {
        DisableAllKeys();
        
        EnabledKeys = count;
        
        for (int i = 0; i < count; i++)
        {
            _keys[i].color = Color.white;
        }
    }

    public void DisableAllKeys()
    {
        EnabledKeys = 0;
        
        for (int i = 0; i < _keys.Length; i++)
        {
            _keys[i].color = new Color(0, 0, 0, 0.5f);
        }
    }
}