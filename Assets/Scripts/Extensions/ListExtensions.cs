using System.Collections.Generic;
using UnityEngine;

public static class List
{
    public static T RandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T RandomElement<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static bool SequenceKeys<T>(this List<T> list, List<T> compare)
    {
        if (compare.Count == 0) return true;

        var result = new HashSet<T>(list);
        result.IntersectWith(compare);

        return result.Count >= compare.Count;
    }
}