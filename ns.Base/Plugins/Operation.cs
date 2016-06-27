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

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        public Operation() : base() {
            DisplayName = "Operation";
            AddChild(new DeviceProperty(nameof(CaptureDevice)));
            AddChild(new StringProperty("LinkedOperation", false));
            AddChild(new ListProperty("Trigger", Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList()));
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
                DeviceProperty property = GetProperty(nameof(CaptureDevice)) as DeviceProperty;
                Device device = property?.SelectedItem;
                if (device != null && device != value && property != null) {
                    device = value;
                    property.SelectedItem = device;
                    OnPropertyChanged();
                }
            }
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
            return base.Finalize();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _captureDevice = (GetProperty(nameof(CaptureDevice)) as DeviceProperty)?.SelectedItem;

            foreach (Tool tool in Childs.Where(t => t is Tool)) {
                if (!tool.Initialize())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            return RunChilds();
        }

        public void AddDeviceList(List<Device> devices) {
            (GetProperty(nameof(CaptureDevice)) as DeviceProperty).Value = devices;
        }
    }
}