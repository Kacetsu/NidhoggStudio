using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class DoubleProperty : NumberProperty<double> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public DoubleProperty(DoubleProperty other) : base(other) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        public DoubleProperty()
            : base() {
            Max = double.MaxValue;
            Min = double.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public DoubleProperty(double value, double min = double.MinValue, double max = double.MaxValue, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, min, max, direction, name) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public DoubleProperty(double value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, double.MinValue, double.MaxValue, direction, name) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new DoubleProperty(this);
    }
}