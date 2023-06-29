using Loxodon.Framework.Binding;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using UnityEngine.UIElements;
using Loxodon.Framework.Observables;
using UnityEngine;
using System;
using Leopotam.Ecs;
using static Keys.Settings;
using static Keys.Sounds;

namespace UIS
{
    public class SettingsView : UIToolkitWindow
    {
        private SettingsViewModel viewModel;
        private IUIViewLocator locator;

        private VisualElement _musicElement;
        private VisualElement _effectsElement;
        private Label _localizationElement;

        private SerializedObservableProperty<float> _music = new SerializedObservableProperty<float>();
        private SerializedObservableProperty<float> _effects = new SerializedObservableProperty<float>();

        protected override void OnCreate(IBundle bundle)
        {
            viewModel = new SettingsViewModel();

            this.locator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            var bindingSet = this.CreateBindingSet(viewModel);

            _musicElement = this.Q<VisualElement>(name: "music__enable");
            bindingSet.Bind(this.Q<Button>(name: "music__button")).
                For(v => v.clickable).
                To(vm => vm.MusicCommand);
            bindingSet.Bind().
                For(v => v.MusicMute).
                To(vm => vm.MusicRequest);

            bindingSet.Bind(this).
                For(v => v._music).
                To(vm => vm.Music);

            _music.ValueChanged += MusicEnable;

            _effectsElement = this.Q<VisualElement>(name: "effects__enable");
            bindingSet.Bind(this.Q<Button>(name: "effects__button")).
                For(v => v.clickable).
                To(vm => vm.EffectsCommand);
            bindingSet.Bind().
                For(v => v.EffectsMute).
                To(vm => vm.EffectsRequest);

            bindingSet.Bind(this).
                For(v => v._effects).
                To(vm => vm.Effects);

            _effects.ValueChanged += EffectsEnable;

            bindingSet.Bind(this.Q<Button>(name: "button__localization")).
                For(v => v.clickable).
                To(vm => vm.LocalizationCommand);
            bindingSet.Bind().
                For(v => v.LocalizationAction).
                To(vm => vm.LocalizationRequest);       
            
            bindingSet.Bind(this.Q<Button>(name: "button__close")).
                For(v => v.clickable).
                To(vm => vm.CloseCommand);
            bindingSet.Bind().
                For(v => v.Close).
                To(vm => vm.CloseRequest);

            bindingSet.Bind(this.Q<Label>(name: "label__localization")).
                For(v => v.text).
                To(vm => vm.Localization);

            bindingSet.Build();
        }

        private void MusicEnable(object sender, EventArgs args)
        {
            _musicElement.style.display = _music.Value == 1 ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void EffectsEnable(object sender, EventArgs args)
        {
            _effectsElement.style.display = _effects.Value == 1 ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void MusicMute(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
        }

        private void EffectsMute(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
        }

        private void LocalizationAction(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
        }

        private void Close(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
            Dismiss();
        }
    }

    public class SettingsViewModel : ViewModelBase
    {
        private float _music;

        public float Music
        {
            get { return _music; }
            set { this.Set<float>(ref _music, value); }
        }

        private float _effects;

        public float Effects
        {
            get { return _effects; }
            set { this.Set<float>(ref _effects, value); }
        }

        private string _localization;

        public string Localization
        {
            get { return _localization; }
            set { this.Set<string>(ref _localization, value); }
        }

        private SystemLanguage _language;

        public SystemLanguage Language
        {
            get { return _language; }
            set 
            { 
                this.Set<SystemLanguage>(ref _language, value);
                UpdateLocalLabel(value);
            }
        }

        private Settings Settings => Data.Instance.Settings;

        public SettingsViewModel()
        {
            Music = Settings.Music;
            Effects = Settings.Effects;
            Language = Settings.CurrentLanguage;

            InitMusicMethod();
            InitEffectMethod();
            InitLocalizationCommand();
            InitCloseCommand();
        }

        private void ClickEffect()
        {
            UISMainLauncher.EmitterECS.CreateNewEntity(new OneShotAudioComponent()
            {
                EventName = CLICK_EFFECT
            });
        }

        #region Music
        private InteractionRequest _musicRequest;
        private SimpleCommand _musicCommand;

        public IInteractionRequest MusicRequest
        {
            get { return _musicRequest; }
        }

        public ICommand MusicCommand
        {
            get { return _musicCommand; }
        }

        public void InitMusicMethod()
        {
            _musicRequest = new InteractionRequest();
            _musicCommand = new SimpleCommand(MusicAction);
        }

        private void MusicAction()
        {
            Music = Settings.Music.Value == 1 ? Settings.Music.Value = 0 : Settings.Music.Value = 1;
            UISMainLauncher.EmitterECS.CreateEntity().Get<AudioSettingsComponent>() = new AudioSettingsComponent
            {
                AudioMode = AudioEnum.Music
            };

            ClickEffect();
            _musicRequest.Raise();
        }
        #endregion
        #region Effects
        private InteractionRequest _effectsRequest;
        private SimpleCommand _effectsCommand;

        public IInteractionRequest EffectsRequest
        {
            get { return _effectsRequest; }
        }

        public ICommand EffectsCommand
        {
            get { return _effectsCommand; }
        }

        public void InitEffectMethod()
        {
            _effectsRequest = new InteractionRequest();
            _effectsCommand = new SimpleCommand(EffectAction);
        }

        private void EffectAction()
        {
            Effects = Settings.Effects.Value == 1 ? Settings.Effects.Value = 0 : Settings.Effects.Value = 1;
            UISMainLauncher.EmitterECS.CreateEntity().Get<AudioSettingsComponent>() = new AudioSettingsComponent
            {
                AudioMode = AudioEnum.Effects
            };

            ClickEffect();
            _effectsRequest.Raise();
        }
        #endregion
        #region Localization 
        private InteractionRequest _localizationRequest;
        private SimpleCommand _localizationCommand;

        public IInteractionRequest LocalizationRequest
        {
            get { return _localizationRequest; }
        }

        public ICommand LocalizationCommand
        {
            get { return _localizationCommand; }
        }

        public void InitLocalizationCommand()
        {
            _localizationRequest = new InteractionRequest();
            _localizationCommand = new SimpleCommand(LocalizationAction);
        }

        private void LocalizationAction()
        {
            if (Settings.CurrentLanguage.Value == SystemLanguage.Russian)
                Settings.CurrentLanguage.Value = SystemLanguage.English;
            else if (Settings.CurrentLanguage.Value == SystemLanguage.English)
                Settings.CurrentLanguage.Value = SystemLanguage.Russian;

            Language = Settings.CurrentLanguage;
            ClickEffect();
            _localizationRequest.Raise();
        }

        private void UpdateLocalLabel(SystemLanguage language)
        {
            Localization = language == SystemLanguage.Russian ? RU : EN;
        }
        #endregion
        #region Close 
        private InteractionRequest _closeRequest;
        private SimpleCommand _closeCommand;

        public IInteractionRequest CloseRequest
        {
            get { return _closeRequest; }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public void InitCloseCommand()
        {
            _closeRequest = new InteractionRequest();
            _closeCommand = new SimpleCommand(CloseAction);
        }

        private void CloseAction()
        {
            ClickEffect();
            _closeRequest.Raise();
        }
        #endregion
    }
}