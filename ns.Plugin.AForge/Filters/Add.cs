using ns.Base.Attribute;
using ns.Base.Extensions;
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
    public sealed class Add : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private ImageProperty _imageOverlay;

        /// <summary>
        /// Initializes a new instance of the <see cref="Add"/> class.
        /// </summary>
        public Add() {
            DisplayName = "AForge Add";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOverlay", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Add"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Add(Add other) : base(other) { }

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
                return "The add filter takes two images (source and overlay images)\n"
                    + "of the same size and pixel format and produces an image,\n"
                    + "where each pixel equals to the sum value of corresponding pixels from provided images"
                    + "(if sum is greater than maximum allowed value, 255 or 65535, then it is truncated to that maximum).\n"
                    + "The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp color images for processing.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new Add(this);

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _imageOverlay = GetProperty<ImageProperty>("ImageOverlay");
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
                ImageContainer overlayContainer = _imageOverlay.Value.DeepClone();

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);
                Bitmap overlay = Converter.ToBitmap(overlayContainer.Data, overlayContainer.Width, overlayContainer.Height, overlayContainer.Stride, pixelFormat);

                global::AForge.Imaging.UnmanagedImage uSource = global::AForge.Imaging.UnmanagedImage.FromManagedImage(source);
                global::AForge.Imaging.UnmanagedImage uOverlay = global::AForge.Imaging.UnmanagedImage.FromManagedImage(overlay);

                AFilter.Add filter = new AFilter.Add(uOverlay);
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