using System;
using UnityEngine;
using Loxodon.Framework.Observables;

[Serializable]
public class Data : ConstructSingleton<Data>
{
    public Settings Settings;
    public Statistic Statistic;
    public Person Person;
    public MiniGameInfo MiniGameInfo;
    public Stock Stock;
    public GameResources Resources;

    public Data()
    {
        Stock = new Stock()
        {
            Items = new SerializedObservableList<Item>(),
        };

        Resources = new GameResources()
        {
            Gold = new SerializedObservableProperty<float> { Value = 1000},
        };

        Statistic = new Statistic()
        {
            LastActivity = DateTime.UtcNow,
        };

        Settings = new Settings()
        {
            DefaultLanguage = SystemLanguage.English,
            CurrentLanguage = new SerializedObservableProperty<SystemLanguage> { Value = SystemLanguage.Russian },
            Music = new SerializedObservableProperty<float> { Value = 1},
            Effects = new SerializedObservableProperty<float> { Value = 1},
        };

        Person = new Person() 
        {
            Level = 0,
        };

        MiniGameInfo = new MiniGameInfo()
        {
            HealthCount = -1,
            StageCount = -1,
            CurrentStageID = -1,
            StopGame = false,
            CurrentStage = default,
        };
    }
}

[Serializable]
public struct Stock
{
    public SerializedObservableList<Item> Items;
}

[Serializable]
public struct GameResources
{
    public SerializedObservableProperty<float> Gold;
}

[Serializable]
public struct Settings
{
    public SystemLanguage DefaultLanguage;
    public SerializedObservableProperty<SystemLanguage> CurrentLanguage;
    public SerializedObservableProperty<float> Music;
    public SerializedObservableProperty<float> Effects;
}

[Serializable]
public struct Statistic
{
    public long Ticks;

    public DateTime LastActivity
    {
        get => new DateTime(Ticks);
        set => Ticks = value.Ticks;
    }
}

[Serializable]
public struct Person
{
    public int Level;
}

[Serializable]
public class MiniGameInfo
{
    public SerializedObservableProperty<int> HealthCount;
    public SerializedObservableProperty<int> StageCount;
    public SerializedObservableProperty<int> CurrentStageID;
    public bool StopGame;
    public StageInfo CurrentStage;
}