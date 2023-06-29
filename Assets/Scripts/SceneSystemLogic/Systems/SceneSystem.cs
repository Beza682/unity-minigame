using Leopotam.Ecs;
using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly EcsFilter<LoadSceneComponent> _loadSceneFilter = null;
    private readonly EcsFilter<UnloadSceneComponent> _unloadSceneFilter = null;

    private const string MINIGAME_SCENE = "MiniGameScene";

    public static Action<string> LoadScene;

    public void Init()
    {
        SceneManager.LoadScene(MINIGAME_SCENE);
    }

    public void Run()
    {
        Load();
        Unload();
    }

    private void Load()
    {
        foreach (var idx in _loadSceneFilter)
        {
            var entity = _loadSceneFilter.GetEntity(idx);
            var component = _loadSceneFilter.Get1(idx);

            SceneManager.LoadScene(component.SceneName);
            //SceneManager.LoadSceneAsync(component.SceneName);
            //var load = SceneManager.LoadSceneAsync(component.SceneName, LoadSceneMode.Additive);

            //while (!load.isDone)
            //{
            //    await Task.Yield();
            //}

            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(component.SceneName));

            entity.Destroy();
        }
    }

    private void Unload()
    {
        foreach (var idx in _unloadSceneFilter)
        {
            ref var entity = ref _unloadSceneFilter.GetEntity(idx);
            ref var component = ref _unloadSceneFilter.Get1(idx);

            SceneManager.UnloadSceneAsync(component.SceneName);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(MINIGAME_SCENE));

            entity.Destroy();
        }
    }
}