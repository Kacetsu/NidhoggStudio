using System;
using System.Collections.Generic;
using System.Linq;
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
        public DeviceContainerProperty(DeviceContainerProperty other) : base(other) {
            SelectableDevices = other.SelectableDevices;
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public Type FilterType => _filterType;

        /// <summary>
        /// Gets the selectable devices.
        /// </summary>
        /// <value>
        /// The selectable devices.
        /// </value>
        public ICollection<Device> SelectableDevices { get; private set; } = new List<Device>();

        /// <summary>
        /// Gets the selected device.
        /// </summary>
        /// <value>
        /// The selected device.
        /// </value>
        public Device SelectedDevice { get { return SelectableDevices?.FirstOrDefault(d => d.Id.Equals(Value?.Id)); } }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type => typeof(List<Device>);

        /// <summary>
        /// Adds the devices.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public void AddDevices(ICollection<Device> devices) {
            if (SelectableDevices == null) {
                SelectableDevices = new List<Device>();
            }

            foreach (Device device in devices) {
                Device tmpDevice = new Device(device);
                tmpDevice.Id = device.Id;
                tmpDevice.Items.Clear();

                foreach (Node node in device.Items) {
                    tmpDevice.Items.Add(node);
                }

                Device matchDevice = Items.FirstOrDefault(d => d.Name.Equals(tmpDevice.Name)) as Device;

                if (matchDevice == null) {
                    Items.Add(tmpDevice);
                } else {
                    device.Id = matchDevice.Id;
                }

                SelectableDevices.Add(device);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new DeviceContainerProperty(this);
    }
}