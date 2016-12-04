using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    [KnownType(typeof(NumberProperty<object>))]
    public abstract class GenericProperty<T> : Property, IValue<T> {
        private T _initialValue;
        private T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        protected GenericProperty()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        protected GenericProperty(T value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(direction, name) {
            Value = value;
            _initialValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericProperty{T}"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        protected GenericProperty(GenericProperty<T> other) : base(other) {
            if (other == null) throw new ArgumentException(nameof(other));

            Value = other.Value;
            _initialValue = other._initialValue;
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type => Value.GetType();

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
        /// Parents the property changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs" /> instance containing the event data.</param>
        protected override void ConnectedPropertyChangedHandle(object sender, PropertyChangedEventArgs e) {
            if (e == null) throw new ArgumentException(nameof(e));

            if (e.PropertyName.Equals(nameof(Value))) {
                IValue valueProperty = ConnectedProperty as IValue;
                if (valueProperty != null) {
                    Value = (T)valueProperty.ValueObj;
                }
            }
        }
    }
}