using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;

namespace ns.Plugin.Base {

    [Visible, Serializable]
    public class GetImage : Tool {
        private ImageDevice _device;
        private ImageProperty _imageProperty;
        private DeviceProperty _deviceProperty;
        private bool _isRunning = false;

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category {
            get {
                return ToolCategory.Acquisition.GetDescription();
            }
        }

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description {
            get {
                return "Configurates the device and acquires a images.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetImage"/> class.
        /// </summary>
        public GetImage() {
            DisplayName = "Get Image";
            AddChild(new DeviceProperty("Interface", "Device", typeof(ImageDevice)));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            bool result = false;
            _deviceProperty = GetProperty("Interface") as DeviceProperty;
            if (_deviceProperty == null) {
                Trace.WriteLine("Device interface is null!", LogCategory.Error);
                return false;
            }
            _deviceProperty.PropertyChanged -= DeviceProperty_PropertyChanged;
            _deviceProperty.PropertyChanged += DeviceProperty_PropertyChanged;
            _device = _deviceProperty.Value as ImageDevice;
            if (_device == null) {
                Trace.WriteLine("Device is null!", LogCategory.Error);
                return false;
            }
            _imageProperty = GetProperty(typeof(ImageProperty)) as ImageProperty;

            if (_device == null) {
                Trace.WriteLine("Device interface is null!", LogCategory.Error);
            } else {
                result = _device.Initialize();
                if (!result)
                    Trace.WriteLine("Could not initialize device!", LogCategory.Error);
            }

            _isRunning = result;
            return result;
        }

        private void DeviceProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Device" && _isRunning) {
                this.Finalize();
                this.Initialize();
            }
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool PreRun() {
            bool result = true;
            if (_isRunning)
                result = base.PreRun() && _device.PreRun();
            return result;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            bool result = false;
            if (!_isRunning)
                return true;
            if (_device != null) {
                lock (_device) {
                    if (result = _device.Run()) {
                        ImageProperty deviceImage = _device.GetProperty(typeof(ImageProperty)) as ImageProperty;
                        _imageProperty.Value = deviceImage.Value;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool PostRun() {
            bool result = true;
            if (_isRunning)
                result = base.PostRun() && _device.PostRun();
            return result;
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            _isRunning = false;
            if (_device != null) {
                lock (_device) {
                    _device.Finalize();
                    _device = null;
                }
            }
            if (_imageProperty != null)
                _imageProperty = null;
            return base.Finalize();
        }
    }
}