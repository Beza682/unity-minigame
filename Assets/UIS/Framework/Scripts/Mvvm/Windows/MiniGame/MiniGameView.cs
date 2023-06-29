using Loxodon.Framework.Binding;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Views;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Commands;
using UnityEngine.UIElements;
using UnityEngine;
using Loxodon.Framework.Observables;
using System;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections.Specialized;
using static Keys.Sounds;

namespace UIS
{
    public class MiniGameView : UIToolkitWindow
    {
        private MiniGameViewModel viewModel;
        private IUIViewLocator locator;

        private VisualElement _startMenu;
        private VisualElement _gameMenu;
        private VisualElement _endMenu;

        private VisualElement _rewardIconPanel;
        private VisualElement _rewardIcon;
        private Label _rewardText;

        private Button _restartMenu;
        private Button _closeMenu;

        private VisualElement _heartsContainer;

        private int _stageCount;
        private SerializedObservableProperty<bool> _hit = new SerializedObservableProperty<bool>();
        private SerializedObservableProperty<int> _heartsCount = new SerializedObservableProperty<int>();
        private SerializedObservableProperty<Item> _item = new SerializedObservableProperty<Item>();

        protected override void OnCreate(IBundle bundle)
        {
            viewModel = new MiniGameViewModel();

            this.locator = Context.GetApplicationContext().GetService<IUIViewLocator>();
            var bindingSet = this.CreateBindingSet(viewModel);

            _startMenu = this.Q<VisualElement>(name: "MiniGameStartMenu");
            _gameMenu = this.Q<VisualElement>(name: "MiniGamePlayMenu");
            _endMenu = this.Q<VisualElement>(name: "MiniGameEndMenu");

            _rewardIconPanel = this.Q<VisualElement>(name: "reward__icon--panel");
            _rewardIcon = this.Q<VisualElement>(name: "reward__icon");
            _rewardText = this.Q<Label>(name: "reward_text");

            bindingSet.Bind(this.Q<Button>(name: "button__settings")).
                For(v => v.clickable).
                To(vm => vm.SettingsCommand);
            bindingSet.Bind().
                For(v => v.OnSettingsWindow).
                To(vm => vm.SettingsRequest);

            _heartsContainer = this.Q<VisualElement>(name: "hearts__body");
            bindingSet.Bind(this.Q<Button>(name: "button__start-game")).
                For(v => v.clickable).
                To(vm => vm.StartCommand);
            bindingSet.Bind().
                For(v => v.StartGame).
                To(vm => vm.StartRequest);

            bindingSet.Bind(this.Q<Button>(name: "button__close-game")).
                For(v => v.clickable).
                To(vm => vm.CloseCommand);
            bindingSet.Bind().
                For(v => v.CloseGame).
                To(vm => vm.CloseRequest);

            bindingSet.Bind(this.Q<Button>(name: "button__shot")).
                For(v => v.clickable).
                To(vm => vm.ShotCommand);
            bindingSet.Bind().
                For(v => v.Shot).
                To(vm => vm.ShotRequest);

            _restartMenu = this.Q<Button>(name: "button__next-game");
            bindingSet.Bind(_restartMenu).
                For(v => v.clickable).
                To(vm => vm.NewGameCommand);
            bindingSet.Bind().
                For(v => v.NextGame).
                To(vm => vm.NewGameRequest);

            _closeMenu = this.Q<Button>(name: "button__end-game");
            bindingSet.Bind(_closeMenu).
                For(v => v.clickable).
                To(vm => vm.CloseCommand);
            bindingSet.Bind().
                For(v => v.CloseGame).
                To(vm => vm.CloseRequest);

            bindingSet.Bind(this).
                For(v => v._heartsCount).
                To(vm => vm.Health);

            _heartsCount.ValueChanged += UpdateHealth;
            _heartsCount.ValueChanged += EndGame;

            bindingSet.Bind(this).
                For(v => v._stageCount).
                To(vm => vm.StageCount);

            bindingSet.Bind(this).
                For(v => v._hit).
                To(vm => vm.Hit);

            _hit.ValueChanged += NewGame;

            bindingSet.Bind(this).
                For(v => v._item).
                To(vm => vm.Item);

            _item.ValueChanged += SetItem;

            ChangeMenu(start: DisplayStyle.Flex);

            bindingSet.Build();
        }

        protected void OnSettingsWindow(object sender, InteractionEventArgs args)
        {
            Debug.Log("Window: Settings");
            IWindow window = locator.LoadWindow<SettingsView>(this.WindowManager, "UIPrefabs/SettingsWindow");
            window.Show();
            window.Create();
        }

        protected void StartGame(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
            SetHealth((int)args.Context);
            ChangeMenu(game: DisplayStyle.Flex);
        }

        protected void CloseGame(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
        }

        protected void NextGame(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();

            ChangeMenu(start: DisplayStyle.Flex);
        }

        protected void Shot(object sender, InteractionEventArgs args)
        {
            args.Callback?.Invoke();
        }

        private void SetHealth(in int health)
        {
            byte idx = 0;
            foreach (var heart in _heartsContainer.Children())
            {
                if (idx < health)
                    heart.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                else
                    heart.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                idx++;
            }
        }

        private void UpdateHealth(object sender, EventArgs args)
        {
            byte idx = 0;
            foreach (VisualElement stackElement in _heartsContainer.Children())
            {
                foreach (VisualElement heart in stackElement.Children())
                {
                    if (idx < _heartsCount)
                    //if (idx < (SerializedObservableProperty<int>)sender)
                        heart.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    else
                        heart.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    idx++;
                }
            }
        }

        private void SetItem(object sender, EventArgs args)
        {
            if(_item.Value != null)
            {
                _rewardIconPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _rewardIcon.style.backgroundImage = new StyleBackground(_item.Value.Sprite);
                _rewardText.text = $"Congratulations! You won <color=red><b>{_item.Value.Name}</b></color>.";
            }
        }

        private void NewGame(object sender, EventArgs args)
        {
            if (_hit)
            //if ((SerializedObservableProperty<bool>)sender)
            {
                ChangeMenu(end: DisplayStyle.Flex);

                if (_stageCount > 0)
                    _restartMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                else
                    _restartMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
        }

        private void EndGame(object sender, EventArgs args)
        {
            if (_heartsCount == 0)
            //if ((SerializedObservableProperty<int>)sender == 0)
            {
                ChangeMenu(end: DisplayStyle.Flex);
                _restartMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);

                if (_item.Value == null)
                {
                    _rewardIconPanel.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _rewardText.text = "You Lose!";
                }
            }
        }

        private void ChangeMenu(DisplayStyle start = DisplayStyle.None, DisplayStyle game = DisplayStyle.None, DisplayStyle end = DisplayStyle.None)
        {
            _startMenu.style.display = new StyleEnum<DisplayStyle>(start);
            _gameMenu.style.display = new StyleEnum<DisplayStyle>(game);
            _endMenu.style.display = new StyleEnum<DisplayStyle>(end);
        }
    }

    public class MiniGameViewModel : ViewModelBase
    {
        private MiniGameInfo MimicGameInfo => Data.Instance.MiniGameInfo;
        private SerializedObservableList<Item> Stock => Data.Instance.Stock.Items;

        private Item _item;
        public Item Item
        {
            get { return _item; }
            set { this.Set<Item>(ref _item, value); }
        }

        private int _health;
        public int Health
        {
            get { return _health; }
            set { this.Set<int>(ref _health, value); }
        }

        private int _stageCount;
        public int StageCount
        {
            get { return _stageCount; }
            set { this.Set<int>(ref _stageCount, value); }
        }

        private bool _hit;
        public bool Hit
        {
            get { return _hit; }
            set { this.Set<bool>(ref _hit, value); }
        }

        public MiniGameViewModel()
        {
            MimicGameInfo.HealthCount.ValueChanged += HealthUpdated;
            MimicGameInfo.StageCount.ValueChanged += StageCountUpdated;
            Stock.CollectionChanged += HitUpdater;
            Health = MimicGameInfo.HealthCount.Value;
            StageCount = MimicGameInfo.StageCount.Value;
            Hit = false;

            InitSettingsCommand();
            InitStartCommand();
            InitCloseCommand();
            InitRestartCommand();
            InitNewGameCommand();
            InitShotCommand();
        }

        private void HealthUpdated(object sender, EventArgs args)
        {
            Health = (SerializedObservableProperty<int>)sender;

            if(Health == 0)
            {
                MimicGameInfo.StopGame = true;
                Item = null;
            }
        }

        private void StageCountUpdated(object sender, EventArgs args)
        {
            StageCount = (SerializedObservableProperty<int>)sender;
        }

        private void HitUpdater(object sender, NotifyCollectionChangedEventArgs args)
        {
            if(args.NewItems[0] is Item item)
                Item = item;
            MimicGameInfo.StopGame = true;
            Hit = true;
        }

        private void ClickEffect()
        {
            UISMainLauncher.EmitterECS.CreateNewEntity(new OneShotAudioComponent()
            {
                EventName = CLICK_EFFECT
            });
        }

        #region Settings 
        private InteractionRequest _settingsRequest;
        private SimpleCommand _settingsCommand;

        public IInteractionRequest SettingsRequest
        {
            get { return _settingsRequest; }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand; }
        }

        public void InitSettingsCommand()
        {
            _settingsRequest = new InteractionRequest();
            _settingsCommand = new SimpleCommand(SettingsAction);
        }

        private void SettingsAction()
        {
            ClickEffect();
            _settingsRequest.Raise();
        }
        #endregion
        #region Start
        private InteractionRequest<int> _startRequest;
        private SimpleCommand _startCommand;

        public IInteractionRequest StartRequest
        {
            get { return _startRequest; }
        }

        public ICommand StartCommand
        {
            get { return _startCommand; }
        }

        public void InitStartCommand()
        {
            _startRequest = new InteractionRequest<int>();
            _startCommand = new SimpleCommand(StartGame);
        }

        private void StartGame()
        {
            UISMainLauncher.EmitterECS.CreateTagEntity<StartGameTag>();
            MimicGameInfo.StopGame = false;

            ClickEffect();
            _startRequest.Raise(Health);
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
            _closeCommand = new SimpleCommand(EndGame);
        }
        private void EndGame()
        {
            //TODO дикий костыль, но он возник из-за проблемы с сущностями. Переходя на эту сцену,
            //сущности имеющие ссылки на игровые объекты теряют эти ссылки и всё становится плохо. Нужно придумать решение
            DataService.Save(Data.Instance);

            Object.Destroy(GameObject.Find(nameof(EcsGameStartup)));
            SceneManager.LoadScene("LoaderScene", LoadSceneMode.Single);

            ClickEffect();
            _closeRequest.Raise();
        }
        #endregion
        #region Restart 
        private InteractionRequest _restartRequest;
        private SimpleCommand _restartCommand;

        public IInteractionRequest RestartRequest
        {
            get { return _restartRequest; }
        }

        public ICommand RestartCommand
        {
            get { return _restartCommand; }
        }

        public void InitRestartCommand()
        {
            _restartRequest = new InteractionRequest();
            _restartCommand = new SimpleCommand(Restart);
        }

        private void Restart()
        {
            UISMainLauncher.EmitterECS.CreateTagEntity<RestartGameTag>();
            UISMainLauncher.EmitterECS.CreateTagEntity<StartGameTag>();
            MimicGameInfo.StopGame = false;

            ClickEffect();
            _restartRequest.Raise();
        }
        #endregion
        #region NewGame 
        private InteractionRequest _newGameRequest;
        private SimpleCommand _newGameCommand;

        public IInteractionRequest NewGameRequest
        {
            get { return _newGameRequest; }
        }

        public ICommand NewGameCommand
        {
            get { return _newGameCommand; }
        }

        public void InitNewGameCommand()
        {
            _newGameRequest = new InteractionRequest();
            _newGameCommand = new SimpleCommand(NewGame);
        }

        private void NewGame()
        {
            UISMainLauncher.EmitterECS.CreateTagEntity<NewGameTag>();
            MimicGameInfo.StopGame = false;
            Hit = false;
            Item = null;

            ClickEffect();
            _newGameRequest.Raise();
        }

        #endregion
        #region Shot 
        private InteractionRequest _shotRequest;
        private SimpleCommand _shotCommand;

        public IInteractionRequest ShotRequest
        {
            get { return _shotRequest; }
        }

        public ICommand ShotCommand
        {
            get { return _shotCommand; }
        }

        public void InitShotCommand()
        {
            _shotRequest = new InteractionRequest();
            _shotCommand = new SimpleCommand(Shot);
        }

        private void Shot()
        {
            //_mimicGameInfo.StopGame = true;
            UISMainLauncher.EmitterECS.CreateTagEntity<ShotTag>();
            UISMainLauncher.EmitterECS.CreateNewEntity(new OneShotAudioComponent()
            {
                EventName = SHOT_SFX
            });
            _shotRequest.Raise();
        }
        #endregion
    }
}