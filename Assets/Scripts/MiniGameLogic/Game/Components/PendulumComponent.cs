using System;
using UnityEngine;

[Serializable]
public struct PendulumComponent
{
    public GameObject Pendulum;
    public LineRenderer RaycastLine;
    internal RaycastHit2D Hit;
}