using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class DeviceContainerProperty : GenericProperty<Device> {
        private Type _filterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContainerProperty"/> class.
        /// </summary>
        public DeviceContainerProperty() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContainerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DeviceContainerProperty(string name) : base(name, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public DeviceContainerProperty(string name, Device value) : base(name, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContainerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public DeviceContainerProperty(string name, string groupName, Device value) : base(name, value) {
            GroupName = groupName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContainerProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="filterType">Type of the filter.</param>
        public DeviceContainerProperty(string name, string groupName, Type filterType) : base(name, null) {
            _filterType = filterType;
            GroupName = groupName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceContainerProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public DeviceContainerProperty(DeviceContainerProperty other) : base(other) { }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public Type FilterType => _filterType;

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type => typeof(List<Device>);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new DeviceContainerProperty(this);
    }
}