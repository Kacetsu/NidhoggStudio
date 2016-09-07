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
    public sealed class ConservativeSmoothing : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _kernelSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        public ConservativeSmoothing() {
            DisplayName = "AForge Conservative Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("KernelSize", 7, 3, 25));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConservativeSmoothing"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ConservativeSmoothing(ConservativeSmoothing other) : base(other) { }

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
                return "The filter implements conservative smoothing,\n"
                    + "which is a noise reduction technique that derives its name from the fact that it employs a simple,\n"
                    + "fast filtering algorithm that sacrifices noise suppression power in order to preserve the high spatial\n"
                    + "frequency detail (e.g. sharp edges) in an image. It is explicitly designed to remove noise spikes -\n"
                    + "isolated pixels of exceptionally low or high pixel intensity (salt and pepper noise).\n"
                    + "The filter accepts 8 bpp grayscale images and 24/32 bpp color images for processing.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new ConservativeSmoothing(this);

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _kernelSize = GetProperty<IntegerProperty>("KernelSize");
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

                AFilter.ConservativeSmoothing filter = new AFilter.ConservativeSmoothing();
                filter.KernelSize = _kernelSize.Value;
                Bitmap destination = filter.Apply(source);

                _imageOutput.Value = Converter.ToImageContainer(destination as Bitmap);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }

            return true;
        }
    }
}