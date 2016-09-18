using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {

    [Visible, DataContract]
    public sealed class BayerFilter : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        /// <summary>
        /// Initializes a new instance of the <see cref="BayerFilter"/> class.
        /// </summary>
        public BayerFilter() {
            DisplayName = "AForge Bayer Filter";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BayerFilter"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public BayerFilter(BayerFilter other) : base(other) { }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description {
            get {
                return "The class implements Bayer filter routine, which creates color image\n"
                    + "out of grayscale image produced by image sensor built with Bayer color matrix.\n"
                    + "TThe filter accepts 8 bpp grayscale images and produces 24 bpp RGB image.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new BayerFilter(this);

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _imageOutput = GetProperty<ImageProperty>("ImageOutput");
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            try {
                ImageContainer inputContainer = _imageInput.Value;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, inputContainer.BytesPerPixel);

                AFilter.BayerFilterOptimized filter = new AFilter.BayerFilterOptimized();
                Bitmap destination = filter.Apply(source);

                ImageContainer outputContainer = Converter.ToImageContainer(destination);

                _imageOutput.Value = outputContainer;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }

            return true;
        }
    }
}