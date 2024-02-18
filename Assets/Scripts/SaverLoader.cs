using System.Collections.Generic;
using Eiko.YaSDK.Data;
using UnityEngine;

public static class SaverLoader
{
    private static readonly string Path = string.Concat(Application.persistentDataPath, "/Player.json");
    
    public static Player LoadOrCreate(PlayerTowerTemplate template, int level)
    {
        var d = YandexPrefs.GetString("save");
        
        if (d != string.Empty)
        {
            return JsonUtility.FromJson<Player>(d);
        }
        
        return new Player(template, level);

        /*if (YandexPrefs.GetString("Player") != string.Empty)
        {
            var player = new Player(
                YandexPrefs.GetInt("selected"), 
                YandexPrefs.GetInt("level"),
                double.Parse(YandexPrefs.GetString("skinProgress")),
                YandexPrefs.GetInt("rowsCount"), 
                YandexPrefs.GetInt("coins"),
                LoadList("opened"),
                LoadListCell("cells"),
                YandexPrefs.GetInt("buy")
                );

            return player;
        }

        return new Player(template, level);*/
    }

    public static void Save(Player player, PlayerTower playerTower)
    {
        //File.WriteAllText(Path, JsonUtility.ToJson(player));
        
        player._towerCells = new List<ArcherCell>();
        
        foreach (var cell in playerTower.Cells)
        {
            player._towerCells.Add(new ArcherCell(cell.HasArcher, cell.ArcherLevel));
        }

        YandexPrefs.SetString("save", JsonUtility.ToJson(player));
        
        //YandexPrefs.SetString("Player", "1");
        
        //YandexPrefs.SetInt("selected", player.Selected);
        //YandexPrefs.SetInt("level", player.Level);
        //YandexPrefs.SetInt("rowsCount", player.TowerRowsCount);
        //YandexPrefs.SetInt("coins", player.Coins);
        //YandexPrefs.SetInt("buy", player.ArchesBuyPossibilityCount);
        //YandexPrefs.SetString("skinProgress", player.SkinProgress.ToString());
        
        //SaveList("opened", player._opened);
        //SaveListCell("cells", player._towerCells);
    }

    public static void Delete()
    {
        //File.Delete(Path);
        
        YandexPrefs.SetString("save", string.Empty);
    }
}