using ns.Base.Manager;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        public override Type Type {
            get {
                return typeof(string);
            }
        }

        /// <summary>
        /// Gets the type of the filter.
        /// </summary>
        /// <value>
        /// The type of the filter.
        /// </value>
        public Type FilterType {
            get { return _filterType; }
        }

        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device {
            get {
                if (this.Value != null && this.Value is string) {
                    Device device = _devicePlugins.Find(d => d.UID == this.Value as string) as Device;
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
                if (this.Device == null)
                    return _tmpDeviceUID;
                else
                    return this.Device.UID;
            }
        }

        /// <summary>
        /// Gets the device plugins.
        /// </summary>
        /// <value>
        /// The device plugins.
        /// </value>
        public List<Node> DevicePlugins {
            get { return _devicePlugins; }
        }


        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="device">The device.</param>
        public void SetDevice(Node device) {
            _deviceManager.Add(device);
            object oldValue = this.Value;
            this.Value = device;
            if (this.Childs.Count > 0)
                this.Childs[0] = device;
            else
                AddChild(device);
            _tmpDeviceUID = device.UID;
            OnNodeChanged("Device", oldValue);
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void SetDevice(string uid) {
            if (_deviceManager != null) {
                Device device = _deviceManager.Nodes.Find(d => d.UID == uid) as Device;
                if (device != null)
                    this.SetDevice(device);
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
            if (device != null)
                this.SetDevice(device);
            _tmpDeviceUID = uid;
        }

        /// <summary>
        /// Sets the device.
        /// </summary>
        /// <param name="deviceManager">The device manager.</param>
        /// <param name="device">The device.</param>
        public void SetDevice(BaseManager deviceManager, Device device) {
            _deviceManager = deviceManager;
            this.SetDevice(device);
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
            if(_filterType == null)
                _devicePlugins = devicePlugins;
            else {
                _devicePlugins = new List<Node>();

                foreach (Node device in devicePlugins) {
                    if (device.GetType().IsAssignableFrom(FilterType) || device.GetType().IsSubclassOf(FilterType)) {
                        _devicePlugins.Add(device);
                    }
                }
            }

            if (this.Device == null || this.Device.GetType() == typeof(Device)) {
                Device selectedDevice = null;
                if ((selectedDevice = (_devicePlugins.Find(d => d.UID == this.DeviceUID)) as Device) != null) {
                    this.Value = selectedDevice;
                    this.SetDevice(selectedDevice);
                } else if((selectedDevice = (_deviceManager.Nodes.Find(d => d.UID == this.DeviceUID)) as Device) != null) {
                    this.Value = selectedDevice;
                    this.SetDevice(selectedDevice);
                    
                    _devicePlugins.Insert(0, selectedDevice);
                } else if (_devicePlugins.Count > 0) {
                    this.Value = _devicePlugins[0];
                    this.SetDevice(_devicePlugins[0]);
                }
            }
        }
    }
}
