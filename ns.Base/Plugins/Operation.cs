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
                if (_captureDevice != value) {
                    _captureDevice = value;
                    _captureDevice.Parent = this;
                    DeviceProperty deviceProperty = GetProperty<DeviceProperty>(nameof(CaptureDevice));
                    if (deviceProperty == null) {
                        throw new MemberAccessException(nameof(DeviceProperty));
                    }
                    deviceProperty.SelectedItem = value;
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
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            CaptureDevice?.Close();
            base.Close();
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
                CaptureDevice?.Close();
                CaptureDevice = deviceProperty.SelectedItem;
                CaptureDevice?.Initialize();
            }

            _outImageProperty = GetProperty<ImageProperty>("OutImage");

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
        public override bool TryRun() {
            bool result = CaptureDevice?.TryPreRun() == true;

            if (result) {
                CaptureDevice.TryRun();
                ImageProperty deviceImage = CaptureDevice.GetProperty<ImageProperty>();
                _outImageProperty.Value = deviceImage.Value;
                result = TryRunChilds();
            }

            result = CaptureDevice?.TryPostRun() == true;
            return result;
        }

        private void DeviceProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(DeviceProperty.SelectedItem))) {
                CaptureDevice = GetProperty<DeviceProperty>(nameof(CaptureDevice))?.SelectedItem;
            }
        }
    }
}