using UnityEngine;

public abstract class ScriptableArray<T> : ScriptableObject
{
    protected abstract T[] Objects { get; }

    public int Length => Objects.Length;

    public bool HaveItemWithIndex(int index)
    {
        return index - 1 < Objects.Length;
    }

    public T Get(int index)
    {
        return Objects[index - 1];
    }
}