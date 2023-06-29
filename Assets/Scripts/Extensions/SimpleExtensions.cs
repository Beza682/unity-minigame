using UnityEngine;

public static class SimpleExtensions
{
    public static void DeleteChilds(this Transform parent)
    {
        foreach (Transform child in parent)
        {
            Object.Destroy(child.gameObject);
        }
    }
}