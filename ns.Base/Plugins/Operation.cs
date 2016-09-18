using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IntegerProperty _elapsedMsProperty;
        private IntegerProperty _iterationProperty;
        private ImageProperty _outImageProperty;

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        public Operation() : base() {
            DisplayName = "Operation";
            AddChild(new DeviceContainerProperty(nameof(CaptureDevice)));
            AddChild(new StringProperty("LinkedOperation", false));
            AddChild(new ListProperty("Trigger", Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList()));
            IntegerProperty elapsedMsProperty = new IntegerProperty("ElapsedMs", true);
            elapsedMsProperty.Tolerance = new Tolerance<int>(0, 10000);
            AddChild(elapsedMsProperty);
            IntegerProperty iterationProperty = new IntegerProperty("Iterations", true);
            iterationProperty.Tolerance = new Tolerance<int>(0, int.MaxValue);
            AddChild(iterationProperty);
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
            if (other == null) throw new ArgumentNullException(nameof(other));

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
                if (value == null) throw new ArgumentNullException(nameof(value));

                if (_captureDevice != value) {
                    _captureDevice = value;
                    _captureDevice.Parent = this;
                    DeviceContainerProperty deviceProperty = GetProperty<DeviceContainerProperty>(nameof(CaptureDevice));
                    if (deviceProperty == null) {
                        throw new MemberAccessException(nameof(DeviceContainerProperty));
                    }
                    deviceProperty.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Adds the device list.
        /// </summary>
        /// <param name="devices">The devices.</param>
        public void AddDeviceList(IEnumerable<Device> devices) {
            GetProperty<DeviceContainerProperty>(nameof(CaptureDevice)).Items = new ObservableList<Node>(devices.Cast<Node>());
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public new Operation Clone() => new Operation(this);

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

            DeviceContainerProperty deviceProperty = GetProperty<DeviceContainerProperty>(nameof(CaptureDevice));

            if (deviceProperty != null) {
                deviceProperty.PropertyChanged += DeviceProperty_PropertyChanged;
                CaptureDevice?.Close();
                CaptureDevice = deviceProperty.Value;
                CaptureDevice?.Initialize();
            }

            _outImageProperty = GetProperty<ImageProperty>("OutImage");
            _elapsedMsProperty = GetProperty<IntegerProperty>("ElapsedMs");
            _iterationProperty = GetProperty<IntegerProperty>("Iterations");

            foreach (Tool tool in Items.Where(t => t is Tool)) {
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            bool result = CaptureDevice?.TryPreRun() == true;

            if (result) {
                CaptureDevice.TryRun();
                ImageProperty deviceImage = CaptureDevice.GetProperty<ImageProperty>();
                _outImageProperty.Value = deviceImage.Value;
                result = TryRunChilds();
            }

            result = CaptureDevice?.TryPostRun() == true;
            _elapsedMsProperty.Value = (int)stopwatch.ElapsedMilliseconds;
            if (_iterationProperty.Value < int.MaxValue) {
                _iterationProperty.Value++;
            } else {
                _iterationProperty.Value = 0;
            }
            return result;
        }

        private void DeviceProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(DeviceContainerProperty.Value))) {
                CaptureDevice = GetProperty<DeviceContainerProperty>(nameof(CaptureDevice))?.Value;
            }
        }
    }
}