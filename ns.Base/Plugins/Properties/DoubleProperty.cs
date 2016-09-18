using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
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
        public DoubleProperty() : base() {
            Max = double.MaxValue;
            Min = double.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public DoubleProperty(string name, double value) : base(name, value, double.MinValue, double.MaxValue) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isOutput">if set to <c>true</c> [is output].</param>
        public DoubleProperty(string name, bool isOutput) : base(name, 0, double.MinValue, double.MaxValue) {
            IsOutput = isOutput;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public DoubleProperty(string name, double value, double min, double max) : base(name, value, min, max) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new DoubleProperty(this);
    }
}