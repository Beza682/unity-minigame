using System;
using UnityEngine;

namespace Loxodon.Framework.Observables
{
    [Serializable]
    public class SerializedObservableProperty<T> : ObservablePropertyBase<T>, IObservableProperty<T>, ISerializationCallbackReceiver
    {
        [SerializeField] private T _data;

        public SerializedObservableProperty() : this(default(T))
        {

        }

        public SerializedObservableProperty(T value) : base(value)
        {

        }

        public virtual T Value
        {
            get { return this._value; }
            set
            {
                if (this.Equals(this._value, value))
                    return;

                this._value = value;
                this.RaiseValueChanged();
            }
        }

        object IObservableProperty.Value
        {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }

        public override string ToString()
        {
            var v = this.Value;
            if (v == null)
                return "";

            return v.ToString();
        }

        public static implicit operator T(SerializedObservableProperty<T> data)
        {
            return data.Value;
        }

        public static implicit operator SerializedObservableProperty<T>(T data)
        {
            return new SerializedObservableProperty<T>(data);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Value = _data;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _data = this.Value;
        }
    }
}