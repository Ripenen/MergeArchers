using UnityEngine;
using UnityEngine.UI;

public class ButtonYield : CustomYieldInstruction
{
    public override bool keepWaiting => _keepWaiting;
    
    private bool _keepWaiting = true;

    public ButtonYield(Button button)
    {
        button.onClick.AddListener(() => _keepWaiting = false);
    }
}