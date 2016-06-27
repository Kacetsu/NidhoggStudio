using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class DeviceProperty : GenericProperty<List<Device>>, IListProperty<Device> {
        private Type _filterType;
        private List<Device> _list = new List<Device>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        public DeviceProperty() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DeviceProperty(string name) : base(name, new List<Device>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public DeviceProperty(string name, List<Device> values) : base(name, values) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public DeviceProperty(string name, string groupName, List<Device> values) : base(name, values) {
            GroupName = groupName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="filterType">Type of the filter.</param>
        public DeviceProperty(string name, string groupName, Type filterType) : base(name, null) {
            _filterType = filterType;
            GroupName = groupName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public DeviceProperty(DeviceProperty other) : base(other) {
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public Type FilterType => _filterType;

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index {
            get {
                if (Value == null) return -1;
                int index = 0;
                for (; index < _list.Count; index++) {
                    if (_list[index].ToString() == SelectedItem?.ToString()) break;
                    else if (_list[index] == SelectedItem) break;
                }
                return index;
            }
        }

        [DataMember]
        public Device SelectedItem { get; set; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type => typeof(List<Device>);
    }
}