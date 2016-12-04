using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class IntegerProperty : NumberProperty<int> {

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        public IntegerProperty()
            : base() {
            Max = int.MaxValue;
            Min = int.MinValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public IntegerProperty(IntegerProperty other)
            : base(other) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public IntegerProperty(int value, int min = int.MinValue, int max = int.MaxValue, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, min, max, direction, name) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerProperty"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public IntegerProperty(int value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, int.MinValue, int.MaxValue, direction, name) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new IntegerProperty(this);
    }
}