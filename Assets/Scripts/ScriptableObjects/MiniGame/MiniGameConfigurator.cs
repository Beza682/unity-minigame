using System;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo;

//[CreateAssetMenu(fileName = "MiniGameConfigurator", menuName = "Game/MiniGameConfigurator", order = 51)]
public class MiniGameConfigurator : ScriptableObject
{
    public float LineLength;
    [SerializeField] private Platforms[] PlatformsInfo;
    [SerializeField] private PlatformsColor[] PlatformsColor;
    [HideInInspector] public SerializedDictionary<PlatformsSize, ConvertToEntity> PlatformsDict = new SerializedDictionary<PlatformsSize, ConvertToEntity>();
    [HideInInspector] public SerializedDictionary<GamePlatformType, Color> PlatformsColorDict = new SerializedDictionary<GamePlatformType, Color>();

    internal List<Vector3> Vectors = new List<Vector3>() { Vector3.forward, Vector3.back };

    public MiniGamePoolObject[] GamesPool;

    private void OnEnable()
    {
        PlatformsDict.Clear();
        PlatformsColorDict.Clear();

        for (int i = 0; i < PlatformsInfo.Length; i++)
        {
            PlatformsDict[PlatformsInfo[i].PlatformSize] = PlatformsInfo[i].PlatformPref;
        }

        for (int i = 0; i < PlatformsColor.Length; i++)
        {
            PlatformsColorDict[PlatformsColor[i].RewardType] = PlatformsColor[i].PlatformColor;
        }
    }
}

[Serializable]
public struct Platforms
{
    public PlatformsSize PlatformSize;
    public ConvertToEntity PlatformPref;
}

[Serializable]
public struct PlatformsColor
{
    public GamePlatformType RewardType;
    [ColorUsage(false, true)] public Color PlatformColor;
}