using UnityEngine;

[CreateAssetMenu(menuName = "Create GamePrefabs", fileName = "GamePrefabs", order = 0)]
public class GamePrefabs : ScriptableObject
{
    public PlayerTowerTemplate[] PlayerTowers;
    public ParticleSystem Confetti;
    public ChestsTower ChestsTower;
    public Map Map;
}