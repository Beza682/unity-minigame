using Loxodon.Framework.Messaging;
using Loxodon.Framework.Views;

namespace UIS
{
    public class MessageChangeWindow<TView> : MessageBase where TView : UIToolkitWindow
    {
        private string key;

        public MessageChangeWindow(object sender, string keyName) : base(sender)
        {
            key = keyName;
        }

        public string KeyName { get { return key; } }
    }
}