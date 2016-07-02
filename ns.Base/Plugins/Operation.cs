using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class for all Operations.
    /// Instead of all other Base Classes this one can be used directly as a functional Operation.
    /// </summary>
    [DataContract(IsReference = true), KnownType(typeof(OperationTrigger))]
    public class Operation : Plugin {
        private Device _captureDevice;
        private string _linkedOperation = string.Empty;
        private ImageProperty _outImageProperty;

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        public Operation() : base() {
            DisplayName = "Operation";
            AddChild(new DeviceProperty(nameof(CaptureDevice)));
            AddChild(new StringProperty("LinkedOperation", false));
            AddChild(new ListProperty("Trigger", Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList()));
            AddChild(new ImageProperty("OutImage", true));
        }

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        /// <param name="name">The name of the Operation.</param>
        public Operation(string name) : this() {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Operation(Operation other) : base(other) {
            CaptureDevice = other.CaptureDevice;
        }

        /// <summary>
        /// Gets or sets the capture device.
        /// </summary>
        /// <value>
        /// The capture device.
        /// </value>
        public Device CaptureDevice {
            get { return _captureDevice; }
            set {
                DeviceProperty property = GetProperty<DeviceProperty>(nameof(CaptureDevice));
                Device device = property?.SelectedItem;
                if (device != value && property != null) {
                    device?.Finalize();
                    device = value;
                    property.SelectedItem = device;
                    device?.Initialize();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Adds the device list.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public void AddDeviceList(List<Device> devices) {
            GetProperty<DeviceProperty>(nameof(CaptureDevice)).Value = devices;
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Operation clone = this.DeepClone();
            clone.UID = GenerateUID();
            return clone;
        }

        /// <summary>
        /// Finalize the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            return _captureDevice?.Finalize() == true && base.Finalize();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            bool result = base.Initialize();

            DeviceProperty deviceProperty = GetProperty<DeviceProperty>(nameof(CaptureDevice));

            if (deviceProperty != null) {
                deviceProperty.PropertyChanged += DeviceProperty_PropertyChanged;
                _captureDevice = deviceProperty.SelectedItem;
            }
            _outImageProperty = GetProperty<ImageProperty>("OutImage");

            result = _captureDevice?.Initialize() == true;

            if (!result) return result;

            foreach (Tool tool in Childs.Where(t => t is Tool)) {
                if (!(result = tool.Initialize())) {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            bool result = _captureDevice?.PreRun() == true;

            if (result) {
                _captureDevice.Run();
                ImageProperty deviceImage = _captureDevice.GetProperty<ImageProperty>();
                _outImageProperty.Value = deviceImage.Value;
                result = RunChilds();
            }

            result = _captureDevice?.PostRun() == true;
            return result;
        }

        private void DeviceProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(DeviceProperty.SelectedItem))) {
                _captureDevice?.Finalize();
                _captureDevice = GetProperty<DeviceProperty>(nameof(CaptureDevice))?.SelectedItem;
                _captureDevice.Initialize();
            }
        }
    }
}