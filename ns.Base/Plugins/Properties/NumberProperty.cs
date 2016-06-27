using ns.Base.Extensions;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract, KnownType(typeof(DoubleProperty)), KnownType(typeof(IntegerProperty))]
    public abstract class NumberProperty<T> : GenericProperty<T>, INumerical, ITolerance<T>, IConnectable<T> {

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        public NumberProperty() : base() {
            Tolerance = new Tolerance<T>();
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
            Max = max;
            Min = min;
            Tolerance = new Tolerance<T>(min, max);
        }

        /// <summary>
        /// Gets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        [XmlIgnore]
        public T InitialValue { get; set; }

        /// <summary>
        /// Gets if the Property has a numeric value.
        /// </summary>
        public bool IsNumeric => true;

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        [DataMember]
        public T Max { get; set; }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        [DataMember]
        public T Min { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is tolerance disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tolerance disabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsToleranceEnabled => Tolerance != null;

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        [DataMember]
        public Tolerance<T> Tolerance { get; set; }

        /// <summary>
        /// Gets a value indicating whether [in tolerance].
        /// </summary>
        /// <value>
        /// <c>true</c> if [in tolerance]; otherwise, <c>false</c>.
        /// </value>
        public bool InTolerance => (System.Collections.Generic.Comparer<T>.Default.Compare(Max, Value) >= 0) && (System.Collections.Generic.Comparer<T>.Default.Compare(Min, Value) <= 0);

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => this.DeepClone();
    }
}