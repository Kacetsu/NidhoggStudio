using System;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public abstract class GenericProperty<T> : Property {
        private T _value;

        public new T Value {
            get { return _value; }
            set {
                if (_value != null && _value.Equals(value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public GenericProperty() : base() {
        }

        public GenericProperty(string name, T value) : this() {
            Name = name;
            Value = value;
        }

        public GenericProperty(string name, bool isOutput) : base(name, isOutput) {
        }

        public override Type Type {
            get {
                return Value.GetType();
            }
        }
    }
}