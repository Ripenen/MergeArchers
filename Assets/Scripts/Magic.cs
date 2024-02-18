using UnityEngine;

public abstract class Magic : MonoBehaviour, IWithId
{
    public Sprite Icon;
    public int id;

    public int Id => id;
    
    public abstract void Run(PlayerTower tower, LevelTower target, Camera camera, GameFactory factory,
        GameBehaviour behaviour);
}