using ns.Base.Attribute;
using ns.Base.Plugins;
using System;
using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using ns.Base.Log;

namespace ns.Plugin.Base {
    /// <summary>
    /// Calculates the intensity.
    /// </summary>
    [Visible, Serializable]
    public class CheckIntensity : Tool {
        private RectangleProperty _aoiProperty;
        private ImageProperty _inputImage;
        private ImageProperty _outputImage;
        private DoubleProperty _intensityProperty;

        public override string Category {
            get {
                return ToolCategory.Common.GetDescription();
            }
        }

        public override string Description {
            get {
                return "Calculates the intensity. Return value (Intensity) will be in percent.";
            }
        }

        public CheckIntensity() {
            DisplayName = "Check Intensity";
            AddChild(new ImageProperty("InputImage", false));
            AddChild(new RectangleProperty("AOI", 0.0, 0.0, 100.0, 100.0));
            AddChild(new ImageProperty("OuputImage", true));
            DoubleProperty intensityProperty = new DoubleProperty("Intensity", true);
            intensityProperty.Tolerance = new Tolerance<double>(0, 100);
            AddChild(intensityProperty);
        }

        public override bool Initialize() {
            base.Initialize();

            _aoiProperty = GetProperty("AOI") as RectangleProperty;
            _inputImage = GetProperty("InputImage") as ImageProperty;
            _outputImage = GetProperty("OuputImage") as ImageProperty;
            _intensityProperty = GetProperty("Intensity") as DoubleProperty;
            return true;
        }

        /// <summary>
        /// Calculates the intensity from the given aoi.
        /// </summary>
        /// <returns></returns>
        public override bool Run() {
            try {
                ImageContainer inputContainer = (ImageContainer)_inputImage.Value;
                byte[] data = inputContainer.Data;
                byte bpp = inputContainer.BytesPerPixel;

                int width = inputContainer.Width;
                int height = inputContainer.Height;

                int yOffset = (int)_aoiProperty.Y;
                int xOffset = (int)_aoiProperty.X;
                int aoiWidth = (int)_aoiProperty.Width;
                int aoiHeight = (int)_aoiProperty.Height;

                int sum = 0;
                int count = 0;

                unsafe
                {
                    fixed (byte* ptr = data)
                    {
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

                _intensityProperty.Value = Math.Round((100.0 / 255.0) * (double)(sum / count), 2);
                _outputImage.Value = inputContainer.DeepClone();
            } catch(Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
            return true;
        }
    }
}

