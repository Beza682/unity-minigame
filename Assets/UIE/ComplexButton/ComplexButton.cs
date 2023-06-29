using System;
using Leopotam.Ecs;
using UIS;
using UnityEngine.UIElements;

namespace UIE
{
    public class ComplexButton : VisualElement
    {
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _downEventNameAttribute = new UxmlStringAttributeDescription()
            {
                name = "down-event-name",
                defaultValue = "event:/"
            };

            UxmlStringAttributeDescription _upEventNameAttribute = new UxmlStringAttributeDescription()
            {
                name = "up-event-name",
                defaultValue = "event:/"
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                (ve as ComplexButton)._soundDownEventName = _downEventNameAttribute.GetValueFromBag(bag, cc);   
                (ve as ComplexButton)._soundUpEventName = _upEventNameAttribute.GetValueFromBag(bag, cc);
            }
        }

        public new class UxmlFactory : UxmlFactory<ComplexButton, UxmlTraits> { }

        private string _soundDownEventName;
        private string _soundUpEventName;

        private Clickable _clickable;

        public Action ClickedDown;
        public Action ClickedUp;

        public Clickable clickable
        {
            get
            {
                return _clickable;
            }
            set
            {
                if (_clickable != null && _clickable.target == this)
                {
                    this.RemoveManipulator(_clickable);
                }

                _clickable = value;
                if (_clickable != null)
                {
                    this.AddManipulator(_clickable);
                }
            }
        }

        public event Action clicked
        {
            add
            {
                if (_clickable == null)
                {
                    clickable = new Clickable(value);
                }
                else
                {
                    _clickable.clicked += value;
                }
            }
            remove
            {
                if (_clickable != null)
                {
                    _clickable.clicked -= value;
                }
            }
        }

        public string SoundDownEventName
        {
            get => _soundDownEventName;
            set
            {
                if(value != string.Empty)
                {
                    _soundDownEventName = value;
                }
            }
        }

        public string SoundUpEventName
        {
            get => _soundUpEventName;
            set
            {
                if(value != string.Empty)
                {
                    _soundUpEventName = value;
                }
            }
        }

        public ComplexButton()
        {
            this.AddManipulator(clickable);

            this.RegisterCallback<PointerDownEvent>(DownEvent, TrickleDown.TrickleDown);
            this.RegisterCallback<PointerUpEvent>(UpEvent, TrickleDown.TrickleDown);
        }

        private void DownEvent(PointerDownEvent evt)
        {
            ClickedDown?.Invoke();

            UISMainLauncher.EmitterECS.CreateEntity().Get<OneShotAudioComponent>() = new OneShotAudioComponent()
            {
                EventName = _soundDownEventName
            };
        }

        private void UpEvent(PointerUpEvent evt)
        {
            ClickedUp?.Invoke();

            UISMainLauncher.EmitterECS.CreateEntity().Get<OneShotAudioComponent>() = new OneShotAudioComponent()
            {
                EventName = _soundUpEventName
            };
        }
    }
}