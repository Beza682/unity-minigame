using FMOD.Studio;
using FMODUnity;
using Leopotam.Ecs;
using static Keys.Settings;

public class GameAudioSystem : IEcsInitSystem, IEcsRunSystem
{
    private readonly EcsFilter<AudioSettingsComponent> _settingsFilter = null;
    private readonly EcsFilter<OneShotAudioComponent> _gasComponentFilter = null;

    private readonly StudioEventEmitter _studioEmitter = null;

    private readonly Bus _music = RuntimeManager.GetBus(MUSIC_BUS);
    private readonly Bus _effects = RuntimeManager.GetBus(EFFECTS_BUS);

    private Settings Settings => Data.Instance.Settings;

    public void Init()
    {
        _music.setVolume(Settings.Music);
        _effects.setVolume(Settings.Effects);
    }

    public void Run()
    {
        ChangeSettings();
        PlayOneShot();
    }

    private void ChangeSettings()
    {
        foreach (var idx in _settingsFilter)
        {
            ref var entity = ref _settingsFilter.GetEntity(idx);
            ref var component = ref _settingsFilter.Get1(idx);

            if (component.AudioMode == AudioEnum.Music)
            {
                _music.setVolume(Settings.Music);
                _studioEmitter.Play();
            }
            else if (component.AudioMode == AudioEnum.Effects)
            {
                _effects.setVolume(Settings.Effects);
            }

            entity.Destroy();
        }
    }

    private void PlayOneShot()
    {
        foreach (var idx in _gasComponentFilter)
        {
            ref var entity = ref _gasComponentFilter.GetEntity(idx);
            ref var eventName = ref _gasComponentFilter.Get1(idx);

            if (eventName.EventName != string.Empty)
            {
                RuntimeManager.PlayOneShot(eventName.EventName);
            }

            entity.Destroy();
        }
    }
}
