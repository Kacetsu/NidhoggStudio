using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    [KnownType(typeof(DoubleProperty))]
    [KnownType(typeof(IntegerProperty))]
    public abstract class NumberProperty<T> : GenericProperty<T>, INumerical<T>, ITolerance<T>, IConnectable<T> {

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        public NumberProperty()
            : base() {
            Tolerance = new Tolerance<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public NumberProperty(NumberProperty<T> other) : base(other) {
            Tolerance = other.Tolerance;
            Min = other.Min;
            Max = other.Max;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberProperty{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public NumberProperty(T value, T min, T max, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, direction, name) {
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
        public T InitialValue { get; set; }

        /// <summary>
        /// Gets a value indicating whether [in tolerance].
        /// </summary>
        /// <value>
        /// <c>true</c> if [in tolerance]; otherwise, <c>false</c>.
        /// </value>
        public bool InTolerance => (System.Collections.Generic.Comparer<T>.Default.Compare(Tolerance.Max, Value) >= 0) && (System.Collections.Generic.Comparer<T>.Default.Compare(Tolerance.Min, Value) <= 0);

        /// <summary>
        /// Gets if the Property has a numeric value.
        /// </summary>
        public bool IsNumeric => true;

        /// <summary>
        /// Gets a value indicating whether this instance is tolerance disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tolerance disabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsToleranceEnabled => Tolerance != null;

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
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        [DataMember]
        public Tolerance<T> Tolerance { get; set; }
    }
}