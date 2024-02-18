using UnityEngine;

public class GameField
{
    public readonly GameFieldTemplate Field;
    public readonly LevelTower LevelTower;
    public readonly PlayerTower PlayerTower;
    
    public GameField(GameFieldTemplate field, PlayerTower player, LevelTower enemy)
    {
        Field = field;
        PlayerTower = player;
        LevelTower = enemy;
    }

    public GameField(GameField gameFieldInstance, LevelTower enemy)
    {
        Field = gameFieldInstance.Field;
        PlayerTower = gameFieldInstance.PlayerTower;
        LevelTower = enemy;
    }
    
    public void Reload(GameFactory gameFactory)
    {
        PlayerTower.Reload(gameFactory);
        LevelTower.Reload(gameFactory);
    }

    public void Destroy()
    {
        LevelTower.Destroy();
        Object.Destroy(Field.gameObject);
    }
}