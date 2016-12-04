using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class StringProperty : GenericProperty<string> {

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        public StringProperty()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public StringProperty(StringProperty other)
            : base(other) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public StringProperty(string value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, direction, name) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new StringProperty(this);
    }
}