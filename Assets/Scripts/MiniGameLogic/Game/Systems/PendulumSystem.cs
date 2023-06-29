using Leopotam.Ecs;
using UnityEngine;

sealed class PendulumSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly EcsFilter<PendulumComponent> _pendulumFilter = null;
    private readonly EcsFilter<RestartGameTag> _restartFilter = null;
    private readonly EcsFilter<NewGameTag> _newGameFilter = null;

    private readonly MiniGameConfigurator _gameConfig = null;

    private Person Person => Data.Instance.Person;
    public MiniGameInfo MimicGameInfo
    {
        get { return Data.Instance.MiniGameInfo; }
        set { Data.Instance.MiniGameInfo = value; }
    }

    public void Init()
    {
        Start();
    }

    public void Run()
    {
        Restart();
        PendulumSwing();
    }

    private void PendulumSwing()
    {
        if (MimicGameInfo.StopGame) return;

        foreach (var idx in _pendulumFilter)
        {
            ref var pendulum = ref _pendulumFilter.Get1(idx);

            pendulum.Pendulum.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Sin(Time.time * MimicGameInfo.CurrentStage.PendulumSpeed) * _gameConfig.GamesPool[Person.Level].PendulumAmplitude));

            pendulum.Hit = Physics2D.Raycast(pendulum.Pendulum.transform.position, -pendulum.Pendulum.transform.up, _gameConfig.LineLength);

            if (pendulum.Hit)
            {
                pendulum.RaycastLine.enabled = true;
                pendulum.RaycastLine.SetPosition(0, pendulum.Pendulum.transform.position);
                pendulum.RaycastLine.SetPosition(1, pendulum.Hit.point);
            }
            else
            {
                pendulum.RaycastLine.enabled = false;
            }
        }
    }

    private void Restart()
    {
        if (_newGameFilter.IsEmpty() && _restartFilter.IsEmpty()) return;

        Start();
    }

    private void Start()
    {
        foreach (var idx in _pendulumFilter)
        {
            ref var pendulum = ref _pendulumFilter.Get1(idx);

            pendulum.Pendulum.transform.rotation = Quaternion.identity;
            pendulum.Hit = Physics2D.Raycast(pendulum.Pendulum.transform.position, pendulum.Pendulum.transform.position);
            pendulum.RaycastLine.enabled = false;
            pendulum.RaycastLine.SetPosition(0, pendulum.Pendulum.transform.position);
            pendulum.RaycastLine.SetPosition(1, pendulum.Pendulum.transform.position);
        }
    }
}