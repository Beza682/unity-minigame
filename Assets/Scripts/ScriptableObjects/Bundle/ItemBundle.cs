using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBundle", menuName = "ItemBundle", order = 51)]
public class ItemBundle : ScriptableObject
{
    [SerializeField] private List<Item> _items;
    public List<Item> Items { get { return _items; } }
}
