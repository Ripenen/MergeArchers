using UnityEngine;

[CreateAssetMenu(menuName = "Create Magics", fileName = "Magics", order = 0)]
public class MagicContainer : ScriptableObject
{
    public Sprite BombIcon;
    public int BombId;

    public Magic[] Magics;
}