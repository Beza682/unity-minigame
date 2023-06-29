using Leopotam.Ecs;
using Leopotam.Ecs.Ui.Systems;
using System.Collections.Generic;
using UnityEngine;
using Voody.UniLeo;
using UIS;
using FMODUnity;

public sealed class EcsGameStartup : MonoBehaviour
{
    private EcsWorld _world;
    private EcsSystems _systems;

    [SerializeField] private EcsUiEmitter _uiEmitter;
    [SerializeField] private StudioEventEmitter _fmodEmitter;
    [SerializeField] private UISRoot _uisRoot;

    internal static Stack<EcsSystems> StackOfSystems = new Stack<EcsSystems>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectOfType<UISRoot>() == null)
            Instantiate(_uisRoot);

        UISMainLauncher.EmitterECS = _uiEmitter;

        Data.Instance = DataService.Load<Data>();

        _world = new EcsWorld();
        WorldHandler.Init(_world);
        _systems = new EcsSystems(_world);

        _systems.ConvertScene();

        AddSystems();
        AddInjections();
        UiInjections();

        _systems.Init();
        StackOfSystems.Push(_systems);
    }

    private void Update()
    {
        foreach (var systems in StackOfSystems)
        {
            systems?.Run(); 
        }
    }

    private void AddInjections()
    {
        _systems
            .Inject(_fmodEmitter)
            ;
    }

    private void UiInjections()
    {
        _systems
            .InjectUi(_uiEmitter)
            ;
    }

    private void AddSystems()
    {
        _systems
            .Add(new SceneSystem())
            .Add(InitializeEntitySystem())
            ;
    }

    private EcsSystems InitializeEntitySystem()
    {
        return new EcsSystems(_world)
            .Add(new EntityInitializeSystem())
            .OneFrame<InitializeEntityRequest>()
            .Add(new InitEmitterSystem())
            .Add(new GameAudioSystem())
            ;
    }

    private bool _quit;

    private void OnApplicationFocus(bool focus)
    {
        _quit = focus;

        if (!focus) DataService.Save(Data.Instance);
    }

    private void OnApplicationQuit()
    {
        if (_quit)
        {
            Data.Instance.Statistic.LastActivity = System.DateTime.UtcNow;
            DataService.Save(Data.Instance);
        }
    }

    private void OnDestroy()
    {
        if (_systems == null) return;

        StackOfSystems.Clear();
        _systems.Destroy();
        _systems = null;
        WorldHandler.Destroy();
        _world.Destroy();
        _world = null;
    }
}