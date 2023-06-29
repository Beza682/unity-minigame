using Leopotam.Ecs.Ui.Systems;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;

namespace UIS
{
    public static class UISMainLauncher
    {
        private static IMessenger _messenger;

        public static IMessenger Messenger
        {
            get { return _messenger; }
            private set { _messenger = value; }
        }

        private static ISubscription<MessageChangeWindow<MiniGameView>> _miniGameSubscription;

        private static WindowContainer _winContainer;

        public static WindowContainer WinContainer
        {
            get { return _winContainer; }
            private set { _winContainer = value; }
        }

        private static ApplicationContext _context;

        public static ApplicationContext AppContext
        {
            get { return _context; }
            private set { _context = value; }
        }

        public static EcsUiEmitter EmitterECS { get; set; }

        static UISMainLauncher()
        {
            _context = Context.GetApplicationContext();

            BindingServiceBundle bundle = new BindingServiceBundle(_context.GetContainer());
            bundle.Start();

            _context.GetContainer().Register<IUIViewLocator>(new DefaultUIViewLocator());

            _winContainer = WindowContainer.Create("MAIN");

            _messenger = new Messenger();

            _miniGameSubscription = _messenger.Subscribe<MessageChangeWindow<MiniGameView>>(Open);
        }

        public static bool TryGetWindow(string name, out UIToolkitWindow window)
        {
            window = null;

            if (WinContainer.Current == null) return false;

            for (int i = 0; i < WinContainer.Current.WindowManager.Count; i++)
            {
                if (WinContainer.Current.WindowManager.Get(i).Name == name)
                {
                    window = (UIToolkitWindow)WinContainer.Current.WindowManager.Get(i);
                    break;
                }
            }

            return window != null;
        }

        private static void Open<TView>(MessageChangeWindow<TView> message) where TView : UIToolkitWindow
        {
            WinContainer.Clear();

            IUIViewLocator locator = AppContext.GetService<IUIViewLocator>();
            TView window = locator.LoadWindow<TView>(WinContainer, $"UIPrefabs/{message.KeyName}");

            if (window is ILeoSystem system)
                system.SetSystem(message.Sender);

            window.Create();
            window.Show();
        }
    }
}
