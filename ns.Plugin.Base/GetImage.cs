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
                return "Configurates the device and acquires an image.";
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
            _device = (ImageDevice)((DeviceProperty)GetProperty("Interface")).Value;
            _imageProperty = GetProperty(typeof(ImageProperty)) as ImageProperty;

            if (_device == null) {
                Trace.WriteLine("Device interface is null!", LogCategory.Error);
            } else {
                result = _device.Initialize();
            }

            return result;
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool PreRun() {
            base.PreRun();

            return _device.PreRun();
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            bool result = false;

            if (result = _device.Run()) {
                ImageProperty deviceImage = _device.GetProperty(typeof(ImageProperty)) as ImageProperty;
                ImageContainer container = (ImageContainer)deviceImage.Value;
                _imageProperty.SetValue(container);
            }

            return result;
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool PostRun() {
            base.PostRun();

            if (_device != null)
                return _device.PostRun();
            else
                return false;
        }

    }
}
