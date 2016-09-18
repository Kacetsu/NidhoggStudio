using System;

namespace ns.Base.Plugins.Properties {

    public interface IConnectable<T> {

        /// <summary>
        /// Gets a value indicating whether this instance can automatic connect.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can automatic connect; otherwise, <c>false</c>.
        /// </value>
        bool CanAutoConnect { get; }

        /// <summary>
        /// Gets or sets the connected uid.
        /// </summary>
        /// <value>
        /// The connected uid.
        /// </value>
        Guid ConnectedId { get; set; }

        /// <summary>
        /// Gets the connected property.
        /// </summary>
        /// <value>
        /// The connected property.
        /// </value>
        Property ConnectedProperty { get; }

        /// <summary>
        /// Gets or sets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        T InitialValue { get; set; }
    }
}