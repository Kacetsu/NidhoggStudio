using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class StringProperty : GenericProperty<string> {

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        public StringProperty() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public StringProperty(StringProperty other) : base(other) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public StringProperty(string name, string value) : base(name, value) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public StringProperty(string name, bool isOutput) : base(name, isOutput) {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new StringProperty(this);
    }
}