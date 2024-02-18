using System;
using System.Collections.Generic;

[Serializable]
public struct ArcherCell
{
    public bool Item1;
    public int Item2;

    public ArcherCell(bool item1, int item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}

[Serializable]
public class Player
{
    public List<int> _opened = new ();
    public List<int> _openedMagic = new () {0};
    public int Selected;
    public int ArchesBuyPossibilityCount;
    public double SkinProgress;
    public int TowerRowsCount = 2;
    public int Coins;
    public int Level = 1;
    public int KeysCount;
    public int SkipAdCount;
    public int SelectedMagic;
    public bool NoAd;
    public List<ArcherCell> _towerCells;

    public Player(PlayerTowerTemplate selected, int level)
    {
        _opened.Add(selected.Id);
        _openedMagic.Add(0);

        Selected = selected.Id;
        SelectedMagic = 0;
        Level = level;
    }
    
    public bool IsOpened(PlayerTowerTemplate template)
    {
        return _opened.Contains(template.Id);
    }
    
    public void Open(Magic template)
    {
        if (_openedMagic.Contains(template.Id))
            return;
        
        _openedMagic.Add(template.Id);
    }
    
    public void Open(PlayerTowerTemplate template)
    {
        if (_opened.Contains(template.Id))
            return;
        
        _opened.Add(template.Id);
    }
}