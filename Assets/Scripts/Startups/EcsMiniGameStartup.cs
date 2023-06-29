using Leopotam.Ecs;
using UIS;
using UnityEngine;
using Voody.UniLeo;

public class EcsMiniGameStartup : MonoBehaviour
{
    private EcsWorld _world = WorldHandler.GetWorld();
    private EcsSystems _systems;

    [SerializeField] private MiniGameSceneData _miniGameSceneData;
    [SerializeField] private MiniGameConfigurator _miniGameConfigurator;

    private void Awake()
    {
        _systems = new EcsSystems(_world);

        _systems.ConvertScene();

        AddSystems();
        AddInjections();

        _systems.Init();
        EcsGameStartup.StackOfSystems.Push(_systems);

    }

    private void AddInjections()
    {
        _systems
            .Inject(_miniGameSceneData)
            .Inject(_miniGameConfigurator)
            ;
    }

    private void AddSystems()
    {
        _systems
            .Add(new ShotSystem())
            .Add(new MiniGameControlSystem())
            .Add(new SpawnPlatformsSystem())
            .Add(new PendulumSystem())
            .Add(new RotationSystem())
            .OneFrame<SpawnMimicElementTag>()
            .OneFrame<NewGameTag>()
            .OneFrame<RestartGameTag>()
            .OneFrame<ShotTag>()
            .OneFrame<GameOverTag>()
            .OneFrame<StartGameTag>()
            ;
    }

    private void OnDestroy()
    {
        if (_systems == null) return;

        if (EcsGameStartup.StackOfSystems.Count > 0)
        {
            EcsGameStartup.StackOfSystems.Pop();
        }

        UISMainLauncher.WinContainer.Clear();

        _systems.Destroy();
        _systems = null;
    }
}