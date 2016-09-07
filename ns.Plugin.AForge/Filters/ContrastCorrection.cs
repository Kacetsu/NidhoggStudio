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
    public sealed class ContrastCorrection : Tool {
        private IntegerProperty _factor;
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        public ContrastCorrection() {
            DisplayName = "AForge Contrast Correction";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("Factor", 0, -127, 127));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ContrastCorrection(ContrastCorrection other) : base(other) { }

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
                return "The filter operates in RGB color space and adjusts pixels' contrast value\n"
                    + "by increasing RGB values of bright pixel and decreasing RGB values of\n"
                    + "dark pixels (or vise versa if contrast needs to be decreased).\n"
                    + "The filter is based on LevelsLinear filter and simply sets all input ranges to (Factor, 255-Factor)\n"
                    + "and all output range to (0, 255) in the case if the factor value is positive.\n"
                    + "If the factor value is negative, then all input ranges are set to (0, 255 ) and all output ranges are set to (-Factor, 255_Factor).\n"
                    + "See LevelsLinear documentation forr more information about the base filter.\n"
                    + "The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.\n";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() => new ContrastCorrection(this);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _factor = GetProperty<IntegerProperty>("Factor");
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

                AFilter.ContrastCorrection filter = new AFilter.ContrastCorrection(_factor.Value);
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