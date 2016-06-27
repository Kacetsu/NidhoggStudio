using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public abstract class GenericProperty<T> : Property, IValue<T> {
        private T _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public T Value {
            get { return _value; }
            set {
                if (_value != null && _value.Equals(value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the value object.
        /// </summary>
        /// <value>
        /// The value object.
        /// </value>
        public object ValueObj {
            get { return Value; }
            set { Value = (T)value; }
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
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public GenericProperty(GenericProperty<T> other) : base(other) {
            Value = other.Value;
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