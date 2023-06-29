using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Actions;
using Leopotam.Ecs.Ui.Systems;
using UnityEngine;

sealed class InitEmitterSystem : IEcsRunSystem
{
    private readonly EcsFilter<SpawnButton> _spawnUIFilter = null;
    private readonly EcsUiEmitter _ecsUiEmitter = null;

    private GameObject gameObject;
    private string widgetName;

    public void Run()
    {
        AddAction<SpawnButton, EcsUiClickAction>(_spawnUIFilter);
    }

    private void AddAction<TInc1, TAct>(in EcsFilter<TInc1> filter) where TInc1 : struct 
                                                                    where TAct : EcsUiActionBase
    {
        foreach (var i in filter)
        {
            ref var entity = ref filter.GetEntity(i);
            ref var interaction = ref filter.Get1(i);
            var fields = EcsComponentType<TInc1>.Type.GetFields();

            for (int j = 0; j < fields.Length; j++)
            {
                if (fields[j].FieldType == typeof(GameObject))
                {
                    gameObject = fields[j].GetValue(interaction) as GameObject;
                }
                else if (fields[j].FieldType == typeof(string))
                {
                    widgetName = fields[j].GetValue(interaction) as string;
                }
            }
            EcsUiActionBase.AddAction<TAct>(gameObject, widgetName, _ecsUiEmitter);

            entity.Destroy();
        }
    }
}