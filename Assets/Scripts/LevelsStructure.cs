using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Create LevelsStructure", fileName = "LevelsStructure", order = 0)]
public class LevelsStructure : ScriptableArray<Levels>
{
    [SerializeField] private Levels[] _levels;
    protected override Levels[] Objects => _levels;

    public Levels GetLooped(int level, int startLoopValue)
    {
        int length = _levels.Sum(levels => levels.Length);

        while (level >= length)
            level -= length - startLoopValue;

        var d = 0;
        
        for (int i = 0; i < _levels.Length; i++)
        {
            var levelsFromStructure = Get(i + 1);
            
            if (levelsFromStructure.Length + d >= level)
                return levelsFromStructure;

            d += levelsFromStructure.Length;
        }

        throw new ArgumentException();
    }
    
    public TowerTemplate GetLoopedTemplate(int level, int startLoopValue)
    {
        int length = _levels.Sum(levels => levels.Length);

        while (level >= length)
            level -= length - startLoopValue;
        
        var d = 0;
        
        for (int i = 0; i < _levels.Length; i++)
        {
            var levelsFromStructure = Get(i + 1);
            
            if (levelsFromStructure.Length + d >= level)
                return levelsFromStructure.Get(level - d);

            d += levelsFromStructure.Length;
        }

        throw new ArgumentException();
    }
}