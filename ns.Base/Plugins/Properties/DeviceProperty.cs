using ns.Base.Manager;
using System;
using System.Collections.Generic;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public class DeviceProperty : Property {

        [NonSerialized]
        private List<Node> _devicePlugins = new List<Node>();

        [NonSerialized]
        private BaseManager _deviceManager;

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
        public DeviceProperty(string name, string value) : base(name, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public DeviceProperty(string name, string groupName, string value) : base(name, groupName, value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="filterType">Type of the filter.</param>
        public DeviceProperty(string name, string groupName, Type filterType) : base(name, groupName, null) {
            _filterType = filterType;
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
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device {
            get {
                string valueStr = Value as string;
                if (Value != null && !string.IsNullOrEmpty(valueStr)) {
                    Device device = _devicePlugins.Find(d => d.UID == valueStr) as Device;
                    return device;
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the device uid.
        /// </summary>
        /// <value>
        /// The device uid.
        /// </value>
        public string DeviceUID {
            get {
                if (Device == null) return _tmpDeviceUID;
                else return Device.UID;
            }
        }

        /// <summary>
        /// Gets the device plugins.
        /// </summary>
        /// <value>
        /// The device plugins.
        /// </value>
        public List<Node> DevicePlugins => _devicePlugins;

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="device">The device.</param>
        public void SetDevice(Node device) {
            if (Value != null) _deviceManager.Remove(Value as Device);

            _deviceManager.Add(device);
            object oldValue = Value;
            Value = device;

            if (Childs.Count > 0) Childs[0] = device;
            else AddChild(device);

            _tmpDeviceUID = device.UID;
            OnPropertyChanged("Device");
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void SetDevice(string uid) {
            if (_deviceManager != null) {
                Device device = _deviceManager.Nodes.Find(d => d.UID == uid) as Device;
                if (device != null) SetDevice(device);
            }

            _tmpDeviceUID = uid;
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        /// <param name="uid">The uid.</param>
        public void SetDevice(BaseManager deviceManager, string uid) {
            _deviceManager = deviceManager;
            Device device = _deviceManager.Nodes.Find(d => d.UID == uid) as Device;
            if (device != null) SetDevice(device);

            _tmpDeviceUID = uid;
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        /// <param name="device">The device.</param>
        public void SetDevice(BaseManager deviceManager, Device device) {
            _deviceManager = deviceManager;
            SetDevice(device);
        }

        /// <summary>
        /// Adds the device list.
        /// </summary>
        /// <param name="devicePlugins">The device plugins.</param>
        /// <param name="deviceManager">The device manager.</param>
        public void AddDeviceList(List<Node> devicePlugins, BaseManager deviceManager) {
            _deviceManager = deviceManager;

            if (_devicePlugins != null) _devicePlugins.Clear();

            List<Node> _matchingDevices = new List<Node>();
            if (_filterType == null)
                _devicePlugins = devicePlugins;
            else {
                _devicePlugins = new List<Node>();

                foreach (Node device in devicePlugins) {
                    if (device.GetType().IsAssignableFrom(FilterType) || device.GetType().IsSubclassOf(FilterType)) {
                        _devicePlugins.Add(device);
                    }
                }
            }

            Device selectedDevice = null;
            if ((selectedDevice = (_devicePlugins.Find(d => d.UID == DeviceUID)) as Device) != null) {
                Value = selectedDevice;
                SetDevice(selectedDevice);
            } else if ((selectedDevice = (_deviceManager.Nodes.Find(d => d.UID == DeviceUID)) as Device) != null) {
                Value = selectedDevice;
                SetDevice(selectedDevice);

                _devicePlugins.Insert(0, selectedDevice);
            } else if (_devicePlugins.Count > 0) {
                Value = _devicePlugins[0];
                SetDevice(_devicePlugins[0]);
            }
        }
    }
}