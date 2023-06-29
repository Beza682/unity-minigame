using Leopotam.Ecs;

sealed class EntityInitializeSystem : IEcsRunSystem
{
    private readonly EcsFilter<InitializeEntityRequest> _initFilter = null;

    public void Run()
    {
        if (_initFilter.IsEmpty()) return;

        for (int i = 0; i < _initFilter.GetEntitiesCount(); i++)
        {
            ref var entity = ref _initFilter.GetEntity(i);
            ref var request = ref _initFilter.Get1(i);

            request.entityReference.Entity = entity;
        }
    }
}