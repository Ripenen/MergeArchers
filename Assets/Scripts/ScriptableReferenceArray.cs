using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Create ScriptableReferenceArray", fileName = "ScriptableReferenceArray", order = 0)]
public abstract class ScriptableReferenceArray<T> : ScriptableObject where T : Object
{
    protected abstract AssetReferenceT<T>[] Objects { get; }

    public int Length => Objects.Length;

    public bool HaveItemWithIndex(int index)
    {
        return index - 1 < Objects.Length;
    }

    public AssetReferenceT<T> Get(int index)
    {
        return Objects[index - 1];
    }
}