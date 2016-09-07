using ns.Base;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {

    [Visible, DataContract]
    public sealed class ApplyMask : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageMask;
        private ImageProperty _imageOutput;

        public ApplyMask() {
            DisplayName = "AForge Apply Mask";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageMask", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyMask"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ApplyMask(ApplyMask other) : base(other) { }

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
                return "The filter applies mask to the specified image - \n"
                    + "keeps all pixels in the image if corresponding pixels/values of the mask are not equal to 0.\n"
                    + "For all 0 pixels/values in mask, corresponding pixels in the source image are set to 0.\n"
                    + "The filter accepts 8/16 bpp grayscale and 24/32/48/64 bpp color images for processing.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new ApplyMask(this);

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _imageMask = GetProperty<ImageProperty>("ImageMask");
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
                ImageContainer inputContainer = _imageInput.Value.DeepClone();
                ImageContainer overlayContainer = _imageMask.Value.DeepClone();

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, inputContainer.BytesPerPixel);
                Bitmap mask = Converter.ToBitmap(overlayContainer.Data, overlayContainer.Width, overlayContainer.Height, overlayContainer.Stride, inputContainer.BytesPerPixel);

                global::AForge.Imaging.UnmanagedImage uSource = global::AForge.Imaging.UnmanagedImage.FromManagedImage(source);
                global::AForge.Imaging.UnmanagedImage uMask = global::AForge.Imaging.UnmanagedImage.FromManagedImage(mask);

                AFilter.ApplyMask filter = new AFilter.ApplyMask(uMask);
                filter.ApplyInPlace(uSource);

                _imageOutput.Value = inputContainer;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }

            return true;
        }
    }
}