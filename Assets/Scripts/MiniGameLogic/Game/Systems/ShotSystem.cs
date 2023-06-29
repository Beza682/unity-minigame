using Leopotam.Ecs;
using Loxodon.Framework.Observables;
using Voody.UniLeo;

public sealed class ShotSystem : IEcsRunSystem
{
    private readonly EcsWorld _world = null;

    private readonly EcsFilter<ShotTag> _shotFilter = null;
    private readonly EcsFilter<PendulumComponent> _pendulumFilter = null;

    public MiniGameInfo MimicGameInfo
    {
        get { return Data.Instance.MiniGameInfo; }
        set { Data.Instance.MiniGameInfo = value; }
    }   
    
    public Stock Stock
    {
        get { return Data.Instance.Stock; }
        set { Data.Instance.Stock = value; }
    }


    public void Run()
    {
        Shot();
    }

    private void Shot()
    {
        if (_shotFilter.IsEmpty()) return;

        foreach (var pID in _pendulumFilter)
        {
            var result = _pendulumFilter.Get1(pID).Hit.transform.GetComponent<ConvertToEntity>().TryGetEntity();

            if (result.HasValue)
            {
                var platform = result.Value.Get<PlatformInfo>();

                switch (platform.RewardType)
                {
                    case GamePlatformType.Fail:

                        if (MimicGameInfo.HealthCount > 0)
                        {
                            MimicGameInfo.HealthCount.Value--;
                        }
                        break;
                    case GamePlatformType.Reward:
                        ItemBundle rewardInfo = platform.StageReward[MimicGameInfo.CurrentStageID];
                        Item reward = rewardInfo.Items.RandomElement();
                        Stock.Items.Add(reward);

                        break;
                    case GamePlatformType.CriticalReward:
                        ItemBundle critRewardInfo = platform.StageReward[MimicGameInfo.CurrentStageID];
                        Item critReward = critRewardInfo.Items.RandomElement();
                        Stock.Items.Add(critReward);
                        break;
                }
            }
        }
    }

    private void RestartGame()
    {
        _world.NewEntity().Get<RestartGameTag>();
        _world.NewEntity().Get<StartGameTag>();
        MimicGameInfo.StopGame = false;
    }
    private void NewGame()
    {
        _world.NewEntity().Get<NewGameTag>();
        MimicGameInfo.StopGame = false;
    }
}