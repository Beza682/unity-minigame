using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MiniGamePoolObject", menuName = "Game/Mini Game Pool Object", order = 51)]
public class MiniGamePoolObject : ScriptableObject
{
    [SerializeField, Range (10, 45)] private float _pendulumAmplitude = 45;
    [SerializeField, Min(1)] private int _stageCount = 1;
    [SerializeField] private ItemBundle _requestBundle;
    [SerializeField] private List<StageInfo> _stages = new List<StageInfo>();

    public float PendulumAmplitude { get { return _pendulumAmplitude; } }
    public int StageCount { get { return _stageCount; } }
    public ItemBundle RequestBundle { get { return _requestBundle; } }

    public StageInfo GetRandomStage()
    {
        return _stages.FindAll(x => x.Enable == true).RandomElement();
    }
}

[Serializable]
public struct StageInfo
{
    public string Name;
    public bool Enable;
    public float PendulumSpeed;
    public int LifeCount;
    public CirclesInfo[] CirclesList;
}

[Serializable]
public struct CirclesInfo
{
    public float CircleSpeed;
    public MiniGameRotationObject CircleNumber;
    public PlatformInfo[] PlatformsList;
}

[Serializable]
public struct PlatformInfo
{
    [Range(-35f, 35f)]
    public float Angle;
    public PlatformsSize PlatformSize;
    public GamePlatformType RewardType;
    public ItemBundle[] StageReward;
}