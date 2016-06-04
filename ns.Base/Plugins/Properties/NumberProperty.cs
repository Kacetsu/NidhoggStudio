using ns.Base.Extensions;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public class NumberProperty<T> : GenericProperty<T>, INumerical {
        private T _max;
        private T _min;
        private Tolerance<T> _tolerance;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        public NumberProperty() : base() {
            _tolerance = new Tolerance<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public NumberProperty(string name, T value) : base(name, value) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isOutput">if set to <c>true</c> [is output].</param>
        public NumberProperty(string name, bool isOutput) : base(name, isOutput) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public NumberProperty(string name, T value, T min, T max) : base(name, value) {
            _max = max;
            _min = min;
            _tolerance = new Tolerance<T>(min, max);
        }

        /// <summary>
        /// Gets if the Property has a numeric value.
        /// </summary>
        public override bool IsNumeric {
            get { return true; }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public T Max {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public T Min {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tolerance disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tolerance disabled; otherwise, <c>false</c>.
        /// </value>
        public override bool IsToleranceDisabled {
            get { return _tolerance == null; }
        }

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public new Tolerance<T> Tolerance {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            NumberProperty<T> clone = this.DeepClone();
            return clone;
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Save(XmlWriter writer) {
            XmlSerializer ser = new XmlSerializer(typeof(Tolerance<T>));
            ser.Serialize(writer, Tolerance);
        }
    }
}