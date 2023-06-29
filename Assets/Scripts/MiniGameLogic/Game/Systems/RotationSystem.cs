using Leopotam.Ecs;

public sealed class RotationSystem : IEcsRunSystem
{
    private readonly EcsFilter<StartGameTag> _startGameFilter = null;
    private readonly EcsFilter<NewGameTag> _newGameFilter = null;
    private readonly EcsFilter<RotationComponent> _rotationFilter = null;
    private readonly EcsFilter<ChangeRotationComponent> _rotationChangeFilter = null;

    private readonly MiniGameConfigurator _gameConfig = null;

    public MiniGameInfo MimicGameInfo
    {
        get { return Data.Instance.MiniGameInfo; }
        set { Data.Instance.MiniGameInfo = value; }
    }

    public void Run()
    {
        NewGame();
        RotationCircle();
    }

    private void NewGame()
    {
        if (_newGameFilter.IsEmpty() && _startGameFilter.IsEmpty()) return;

        foreach (var idx in _rotationFilter)
        {
            ref var component = ref _rotationFilter.Get1(idx);

            for (int i = 0; i < MimicGameInfo.CurrentStage.CirclesList.Length; i++)
            {
                if (MimicGameInfo.CurrentStage.CirclesList[i].CircleNumber == component.RotationNumber)
                {
                    component.Speed = MimicGameInfo.CurrentStage.CirclesList[i].CircleSpeed;
                }
            }

            component.Vector = _gameConfig.Vectors.RandomElement();
        }
    }

    private void RotationCircle()
    {
        if (MimicGameInfo.StopGame) return;

        foreach (var idx in _rotationFilter)
        {
            ref var component = ref _rotationFilter.Get1(idx);

            if (_rotationChangeFilter.IsEmpty())
            {
                component.RotationObject.transform.Rotate(component.Vector, component.Speed);
            }
            else
            {
                RotationChange(ref component);
            }
        }
    }

    private void RotationChange(ref RotationComponent rotationComponent)
    {
        foreach (var idx in _rotationChangeFilter)
        {
            ref var entity = ref _rotationChangeFilter.GetEntity(idx);
            ref var rotation = ref _rotationChangeFilter.Get1(idx);

            if (rotationComponent.RotationNumber == rotation.RotationObject)
            {
                rotationComponent.Vector = -rotationComponent.Vector;
                entity.Destroy();
            }
        }
    }
}