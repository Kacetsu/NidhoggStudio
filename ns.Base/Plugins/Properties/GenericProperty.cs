using System;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public abstract class GenericProperty<T> : Property, IValue<T> {
        private T _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value {
            get { return _value; }
            set {
                if (_value != null && _value.Equals(value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        public GenericProperty() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public GenericProperty(string name, T value) : this() {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public GenericProperty(string name, bool isOutput) : base(name, isOutput) {
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type {
            get {
                return Value.GetType();
            }
        }
    }
}