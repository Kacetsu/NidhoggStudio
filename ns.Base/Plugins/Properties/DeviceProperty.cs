using ns.Base.Manager;
using System;
using System.Collections.Generic;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public class DeviceProperty : GenericProperty<Device> {

        [NonSerialized]
        private List<Device> _devicePlugins = new List<Device>();

        private string _tmpDeviceUID = string.Empty;
        private Type _filterType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        public DeviceProperty() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public DeviceProperty(string name) : base(name, new Device()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public DeviceProperty(string name, Device value) : base(name, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public DeviceProperty(string name, string groupName, Device value) : base(name, value) {
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
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public DeviceProperty(string name, bool isOutput) : base(name, isOutput) { }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type => typeof(string);

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public Type FilterType => _filterType;

        /// <summary>
        /// Gets the device uid.
        /// </summary>
        /// <value>
        /// The device uid.
        /// </value>
        public string DeviceUID {
            get {
                if (Value == null) return string.Empty;
                else return Value.UID;
            }
        }

        /// <summary>
        /// Gets the device plugins.
        /// </summary>
        /// <value>
        /// The device plugins.
        /// </value>
        public List<Device> DevicePlugins => _devicePlugins;

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="device">The device.</param>
        public void SetDevice(Device device) {
            object oldValue = Value;
            Value = device;

            if (Childs.Count > 0) Childs[0] = device;
            else AddChild(device);

            _tmpDeviceUID = device.UID;
            OnPropertyChanged("Device");
        }

        /// <summary>
        /// Adds the device list.
        /// </summary>
        /// <param name="devicePlugins">The device plugins.</param>
        /// <param name="deviceManager">The device manager.</param>
        public void AddDeviceList(List<Device> devicePlugins) {
            List<Node> _matchingDevices = new List<Node>();
            if (_filterType == null)
                _devicePlugins = devicePlugins;
            else {
                _devicePlugins = new List<Device>();

                foreach (Device device in devicePlugins) {
                    if (device.GetType().IsAssignableFrom(FilterType) || device.GetType().IsSubclassOf(FilterType)) {
                        _devicePlugins.Add(device);
                    }
                }
            }

            Device selectedDevice = null;
            if ((selectedDevice = (_devicePlugins.Find(d => d.UID == DeviceUID)) as Device) != null) {
                Value = selectedDevice;
                SetDevice(selectedDevice);
            } else if (_devicePlugins.Count > 0) {
                Value = _devicePlugins[0];
                SetDevice(_devicePlugins[0]);
            }
        }
    }
}