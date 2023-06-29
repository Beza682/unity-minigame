using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Systems;

public static class EcsExtensions
{
    public static void DestroyEntities(this EcsFilter filter)
    {
        foreach (var idx in filter)
        {
            ref var entity = ref filter.GetEntity(idx);
            entity.Destroy();
        }
    }

    public static void CreateTagEntity<TStruct>(this EcsUiEmitter emitter) where TStruct : struct
    {
        emitter.CreateEntity().Get<TStruct>() = new TStruct();
    }

    public static void CreateNewEntity<TStruct>(this EcsUiEmitter emitter, in TStruct component) where TStruct : struct
    {
        emitter.CreateEntity().Get<TStruct>() = component;
    }
}