using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    /// <summary>
    /// Calculates the intensity.
    /// </summary>
    [Visible, DataContract]
    public sealed class CheckIntensity : Tool {

        public CheckIntensity() : base() {
            DisplayName = "Check Intensity";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckIntensity"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public CheckIntensity(CheckIntensity other) : base(other) {
        }

        /// <summary>
        /// Gets the aoi.
        /// </summary>
        /// <value>
        /// The aoi.
        /// </value>
        public RectangleProperty Aoi => FindOrAdd<RectangleProperty, Rectangle>(new Rectangle(0d, 0d, 100d, 100d));

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category => "Common";

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description => "Calculates the intensity. Return value (Intensity) will be in percent.";

        /// <summary>
        /// Gets the input image.
        /// </summary>
        /// <value>
        /// The input image.
        /// </value>
        public ImageProperty InputImage => FindOrAdd<ImageProperty, ImageContainer>(new ImageContainer());

        /// <summary>
        /// Gets the intensity.
        /// </summary>
        /// <value>
        /// The intensity.
        /// </value>
        public DoubleProperty Intensity => FindOrAdd<DoubleProperty, double>(0d, 0d, 100d, PropertyDirection.Out);

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new CheckIntensity(this);

        /// <summary>
        /// Calculates the intensity from the given aoi.
        /// </summary>
        /// <returns></returns>
        public override bool TryRun() {
            try {
                ImageContainer inputContainer = InputImage.Value;
                byte[] data = inputContainer.Data;
                byte bpp = inputContainer.BytesPerPixel;

                int width = inputContainer.Width;
                int height = inputContainer.Height;

                int yOffset = (int)Aoi.Y;
                int xOffset = (int)Aoi.X;
                int aoiWidth = (int)Aoi.Width;
                int aoiHeight = (int)Aoi.Height;

                int sum = 0;
                int count = 0;

                unsafe
                {
                    fixed (byte* ptr = data) {
                        if (bpp == 1) {
                            for (int y = yOffset; y < (yOffset + aoiHeight) && y < height; y++) {
                                for (int x = xOffset; x < (xOffset + aoiWidth) && x < width; x++) {
                                    byte b = ptr[(y * width + x) * bpp];
                                    sum += b;
                                    count++;
                                }
                            }
                        } else if (bpp >= 3) {
                            for (int y = yOffset; y < (yOffset + aoiHeight) && y < height; y++) {
                                for (int x = xOffset; x < (xOffset + aoiWidth) && x < width; x++) {
                                    byte r = ptr[(y * width + x) * bpp];
                                    byte g = ptr[(y * width + x) * bpp + 1];
                                    byte b = ptr[(y * width + x) * bpp + 2];
                                    sum += r;
                                    sum += g;
                                    sum += b;
                                    count += bpp;
                                }
                            }
                        }
                    }
                }

                Intensity.Value = Math.Round((100.0 / 255.0) * (sum / count), 2);
            } catch (Exception ex) {
                ns.Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
            return true;
        }
    }
}