using System;
using UnityEngine;

[Serializable]
public class Item
{
    [SerializeField] private string _name;
    public string Name { get { return _name; } }

    [SerializeField] private Sprite _sprite;
    public Sprite Sprite { get { return _sprite; } }
}
