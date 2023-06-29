using Leopotam.Ecs;
using UIS;

public sealed class MiniGameControlSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly EcsFilter<NewGameTag> _newGameFilter = null;

    private readonly MiniGameConfigurator _gameConfig = null;

    private int Person => Data.Instance.Person.Level;
    public MiniGameInfo MiniGameInfo
    {
        get { return Data.Instance.MiniGameInfo; }
        set { Data.Instance.MiniGameInfo = value; }
    }

    public void Init()
    {
        var game = _gameConfig.GamesPool[Person].GetRandomStage();

        Data.Instance.MiniGameInfo = new MiniGameInfo()
        {
            CurrentStage = game,
            StageCount = _gameConfig.GamesPool[Person].StageCount - 1,
            CurrentStageID = 0,
            HealthCount = game.LifeCount,
            StopGame = true,
        };

        UISMainLauncher.WinContainer.Clear();
        UISMainLauncher.Messenger.Publish(new MessageChangeWindow<MiniGameView>(this, "MiniGameWindow"));
    }

    public void Run()
    {
        NewGame();
    }

    private void NewGame()
    {
        if (_newGameFilter.IsEmpty()) return;

        var game = _gameConfig.GamesPool[Person].GetRandomStage();

        MiniGameInfo.StageCount.Value--;
        MiniGameInfo.CurrentStageID.Value++;
        MiniGameInfo.CurrentStage = game;
        MiniGameInfo.StopGame = true;
        MiniGameInfo.HealthCount.Value = game.LifeCount;
    }
}