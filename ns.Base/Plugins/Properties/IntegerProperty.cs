using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class IntegerProperty : NumberProperty<int> {

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        public IntegerProperty() : base() {
            Max = int.MaxValue;
            Min = int.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public IntegerProperty(IntegerProperty other) : base(other) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public IntegerProperty(string name, int value) : base(name, value, int.MinValue, int.MaxValue) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isOutput">if set to <c>true</c> [is output].</param>
        public IntegerProperty(string name, bool isOutput) : base(name, 0, int.MinValue, int.MaxValue) {
            IsOutput = isOutput;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public IntegerProperty(string name, int value, int min, int max) : base(name, value, min, max) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new IntegerProperty(this);
    }
}