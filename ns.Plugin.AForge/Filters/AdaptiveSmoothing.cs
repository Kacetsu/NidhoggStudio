using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {

    [Visible, DataContract]
    public sealed class AdaptiveSmoothing : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        public AdaptiveSmoothing() {
            DisplayName = "AForge Adaptive Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveSmoothing"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public AdaptiveSmoothing(AdaptiveSmoothing other) : base(other) { }

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
                return "Adaptive Smoothing - noise removal with edges preserving.\n"
                    + "The filter accepts 8 bpp grayscale images and 24 bpp color images for processing.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new AdaptiveSmoothing(this);

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

                AFilter.AdaptiveSmoothing filter = new AFilter.AdaptiveSmoothing();

                _imageOutput.Value = Converter.ToImageContainer(filter.Apply(source));
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }

            return true;
        }
    }
}