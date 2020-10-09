using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryPool<TKey, TValue>
{
    private static readonly ObjectPool<Dictionary<TKey, TValue>> pool = new ObjectPool<Dictionary<TKey, TValue>>(() => new Dictionary<TKey, TValue>(), null, e => e.Clear());

    public static Dictionary<TKey, TValue> Get()
    {
        return pool.Get();
    }

    public static void Release(Dictionary<TKey, TValue> toRelease)
    {
        pool.Release(toRelease);
    }
}

public static class HashSetPool<TValue>
{
    private static readonly ObjectPool<HashSet<TValue>> pool = new ObjectPool<HashSet<TValue>>(() => new HashSet<TValue>(), null, e => e.Clear());

    public static HashSet<TValue> Get()
    {
        return pool.Get();
    }

    public static void Release(HashSet<TValue> toRelease)
    {
        pool.Release(toRelease);
    }
}