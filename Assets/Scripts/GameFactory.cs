using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class GameFactory
{
    private readonly GamePrefabs _prefabs;
    private readonly ArchersPrefabs _archersPrefabs;
    private readonly LevelsStructure _levels;

    private AssetReference _d;

    public GameFactory(GamePrefabs prefabs, ArchersPrefabs archersPrefabs, LevelsStructure levels)
    {
        _prefabs = prefabs;
        _archersPrefabs = archersPrefabs;
        _levels = levels;
    }
    
    public PlayerTowerTemplate CreatePlayerTower(Transform playerTowerPosition, int id, int rows, List<ArcherCell> cells = null)
    {
        var template = _prefabs.PlayerTowers.First(x => x.Id == id);
        
        var tower = Object.Instantiate(template, playerTowerPosition.position, playerTowerPosition.rotation);
        
        tower.ActivateRows(rows);

        if(cells is not null)
            tower.SetCellsData(cells);
        
        return tower;
    }

    public List<Archer> CreateArchersForPlayer(PlayerTowerTemplate tower)
    {
        return CreateArchersFor(tower, ArcherType.Player, 0);
    }
    
    public List<Archer> CreateArchersForTower(TowerTemplate tower, int level)
    {
        return CreateArchersFor(tower, ArcherType.Enemy, level);
    }
    
    public void CreateMap(Camera camera, int level, MergeView mergeView, InGameView inGameView, Transform back)
    {
        var map = Object.Instantiate(_prefabs.Map, new Vector3(1000, 0, 1000), Quaternion.identity);

        map.Setup(camera, level, mergeView, inGameView, back);
    }

    private List<Archer> CreateArchersFor(TowerTemplate tower, ArcherType type, int level)
    {
        var archerPlaceCells = tower.Cells.Where(x => x.HasArcher);

        var archers = new List<Archer>();

        var prefabs = type == ArcherType.Player ? _archersPrefabs : GetLevelsFromStructure(level).Archers;
        
        foreach (var archerPlaceCell in archerPlaceCells)
        {
            var archer = CreateArcher(type, archerPlaceCell, prefabs);

            archer.transform.parent = tower.transform;
            
            archers.Add(archer);
        }
        
        return archers;
    }

    private Archer CreateArcher(ArcherType type, ArcherPlaceCell archerPlaceCell, ArchersPrefabs prefabs)
    {
        var archerPrefab = prefabs.Get(archerPlaceCell.ArcherLevel);
        
        var archer = Object.Instantiate(archerPrefab, archerPlaceCell.Position,
            archerPlaceCell.transform.rotation);

        archer.ArcherType = type;
        archer.Level = archerPlaceCell.ArcherLevel;
        
        archerPlaceCell.MakeBusy(archer);
        
        return archer;
    }

    public GameFieldTemplate CreateGameField(int level)
    {
        var template = Object.Instantiate(GetLevelsFromStructure(level).FieldTemplate);

        return template;
    }

    public Levels GetLevelsFromStructure(int level)
    {
        return _levels.GetLooped(level, 30);
    }
    
    private TowerTemplate GetLevelFromStructure(int level)
    {
        return _levels.GetLoopedTemplate(level, 30);
    }

    public TowerTemplate CreateLevelTower(int levelId, Transform levelTowerPosition)
    {
        var d = GetLevelFromStructure(levelId);

        d = Object.Instantiate(d, levelTowerPosition.position, levelTowerPosition.rotation);
        
        d.FindCells();

        while (levelId > 50)
            levelId -= 20;
        
        AppMetricaWeb.Event("lvl" + levelId);

        return d;
    }
    
    public bool AddArcher(PlayerTower playerTower, MergingState state)
    {
        var archerPlaceCell = playerTower.Cells.FirstOrDefault(x => !x.HasArcher);

        if (archerPlaceCell)
        {
            var archer = CreateArcher(ArcherType.Player, archerPlaceCell, _archersPrefabs);

            playerTower.MakeParentOf(archer);
            playerTower.AddArcher(archer);

            return true;
        }

        archerPlaceCell = playerTower.Cells.FirstOrDefault(x => x.HasArcher && x.ArcherLevel == 1);

        if (archerPlaceCell)
        {
            state.UniteArchers(archerPlaceCell);

            return true;
        }

        return false;
    }

    public bool HasArcherLevel(int level)
    {
        return _archersPrefabs.HaveItemWithIndex(level);
    }

    public bool GameFieldEquals(GameField gameFieldInstance, int level)
    {
        return GetLevelsFromStructure(level).FieldTemplate.ID == gameFieldInstance.Field.ID;
    }

    [CanBeNull]
    public PlayerTowerTemplate GetRandomNotOpenedPlayerTowerTemplate(Player player)
    {
        for (int i = 0; i < 50; i++)
        {
            var random = _prefabs.PlayerTowers[Random.Range(0, _prefabs.PlayerTowers.Length)];

            if (!player.IsOpened(random))
            {
                return random;
            }
        }

        return null;
    }

    public ParticleSystem CreateConfetti(Vector3 levelTowerCenter)
    {
        return Object.Instantiate(_prefabs.Confetti, levelTowerCenter, Quaternion.Euler(-90, 0, 0));
    }

    public ChestsTower CreateChestTower(Vector3 vector3)
    {
        return Object.Instantiate(_prefabs.ChestsTower, vector3, Quaternion.Euler(-90, 180, 0));
    }
}