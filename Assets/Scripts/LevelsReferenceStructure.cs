using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Create LevelsReferenceStructure", fileName = "LevelsStructure", order = 0)]
public class LevelsReferenceStructure : ScriptableArray<ReferenceLevels>
{
    [SerializeField] private ReferenceLevels[] _levels;
    protected override ReferenceLevels[] Objects => _levels;

    public ReferenceLevels GetLooped(int level, int startLoopValue)
    {
        int length = _levels.Sum(levels => levels.Length);

        while (level > length)
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
    
    public AssetReference GetLoopedTemplate(int level, int startLoopValue)
    {
        int length = _levels.Sum(levels => levels.Length);

        while (level > length)
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