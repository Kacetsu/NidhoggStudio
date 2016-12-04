using ns.Base.Collections;
using ns.Base.Plugins.Devices;
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
    [DataContract(IsReference = true)]
    [KnownType(typeof(OperationTrigger))]
    public class Operation : Plugin {
        private ImageDevice _captureDevice;

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        public Operation()
            : base() {
            DisplayName = "Operation";
        }

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        /// <param name="name">The name of the Operation.</param>
        public Operation(string name)
            : this() {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Operation(Operation other)
            : base(other) {
            if (other == null) throw new ArgumentNullException(nameof(other));
        }

        /// <summary>
        /// Gets or sets the capture device.
        /// </summary>
        /// <value>
        /// The capture device.
        /// </value>
        public ImageDevice CaptureDevice {
            get { return _captureDevice; }
            set {
                if (_captureDevice != value) {
                    _captureDevice = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the execution time ms.
        /// </summary>
        /// <value>
        /// The execution time ms.
        /// </value>
        public IntegerProperty ExecutionTimeMs {
            get {
                IntegerProperty property = FindOrAdd<IntegerProperty, int>(0, PropertyDirection.Out);
                property.Tolerance = new Tolerance<int>(0, 10000);
                return property;
            }
        }

        /// <summary>
        /// Gets the iterations.
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public IntegerProperty Iterations => FindOrAdd<IntegerProperty, int>(0, 0, int.MaxValue, PropertyDirection.Out);

        /// <summary>
        /// Gets or sets the known devices.
        /// </summary>
        /// <value>
        /// The known devices.
        /// </value>
        public ObservableConcurrentDictionary<Guid, string> KnownDevices { get; set; }

        /// <summary>
        /// Gets the linked operation.
        /// </summary>
        /// <value>
        /// The linked operation.
        /// </value>
        public StringProperty LinkedOperation => FindOrAdd<StringProperty, string>(string.Empty);

        /// <summary>
        /// Gets the output image.
        /// </summary>
        /// <value>
        /// The output image.
        /// </value>
        public ImageProperty OutputImage => FindOrAdd<ImageProperty, ImageContainer>(new ImageContainer(), PropertyDirection.Out);

        /// <summary>
        /// Gets or sets the selected device.
        /// </summary>
        /// <value>
        /// The selected device.
        /// </value>
        [DataMember]
        public Guid SelectedDevice { get; set; }

        /// <summary>
        /// Gets the triggers.
        /// </summary>
        /// <value>
        /// The triggers.
        /// </value>
        public ListProperty Triggers => FindOrAdd<ListProperty, List<object>>(Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList());

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
        public override void Initialize() {
            base.Initialize();
            PropertyChanged -= Operation_PropertyChanged;
            PropertyChanged += Operation_PropertyChanged;

            if (CaptureDevice != null) {
                CaptureDevice.Close();
                CaptureDevice.Initialize();
            }

            foreach (Tool tool in Items.Values.OfType<Tool>()) {
                tool.Initialize();
            }
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
                ImageProperty deviceImage = CaptureDevice.OutputImage;
                OutputImage.Value = deviceImage.Value;
                result = TryRunChilds();
            }

            result = CaptureDevice?.TryPostRun() == true;
            ExecutionTimeMs.Value = (int)stopwatch.ElapsedMilliseconds;
            if (Iterations.Value < int.MaxValue) {
                Iterations.Value++;
            } else {
                Iterations.Value = 0;
            }
            return result;
        }

        private void Operation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(CaptureDevice), StringComparison.Ordinal)) {
                Close();
                Initialize();
            }
        }
    }
}