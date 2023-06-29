using Loxodon.Framework.Commands;
using Loxodon.Framework.Interactivity;
using System;
using System.ComponentModel;

namespace UIS
{
    public class VMAction : INotifyPropertyChanged
    {
        private SimpleCommand _command;
        private InteractionRequest _request;
        private Action _action;

        public event PropertyChangedEventHandler PropertyChanged;

        public VMAction()
        {

        }

        public VMAction(params Action[] actions)
        {
            foreach (var action in actions)
            {
                _action += action;
            }
        }

        public void Open()
        {
            _request = new InteractionRequest();
            _command = new SimpleCommand(() =>
            {
                //_action?.Invoke();

                _request.Raise(_action);
            });
        }

        public IInteractionRequest Request
        {
            get { return _request; }
        }

        public ICommand Command
        {
            get { return _command; }
        }
    }

    public interface IReturnable
    {
        public void ReturnTo() { }
    }
}
