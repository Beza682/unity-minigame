using System;
using UnityEngine;

[Serializable]
public struct RotationComponent
{
    public MiniGameRotationObject RotationNumber;
    public GameObject RotationObject;
    internal float Speed;
    internal float Time;
    internal Vector3 Vector;
}