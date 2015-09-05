using ns.Base.Attribute;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using ns.Base.Log;

namespace ns.Plugin.Base {
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
                return string.Empty;
            }
        }

        public CheckIntensity() {
            DisplayName = "Check Intensity";
            AddChild(new RectangleProperty("AOI", 0.0, 0.0, 100.0, 100.0));
            AddChild(new ImageProperty("InputImage", false));
            AddChild(new ImageProperty("OuputImage", true));
            AddChild(new DoubleProperty("Intensity", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _aoiProperty = GetProperty("AOI") as RectangleProperty;
            _inputImage = GetProperty("InputImage") as ImageProperty;
            _outputImage = GetProperty("OuputImage") as ImageProperty;
            _intensityProperty = GetProperty("Intensity") as DoubleProperty;
            return true;
        }

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

                unsafe {
                    fixed(byte* ptr = data) {
                        if (bpp == 1) {
                            for (int y = yOffset; y < (yOffset + aoiHeight); y++) {
                                for (int x = xOffset; x < (xOffset + aoiWidth); x++) {
                                    byte b = ptr[(y * width + x) * bpp];
                                    sum += b;
                                    count++;
                                }
                            }
                        }else if (bpp >= 3) {
                            for (int y = yOffset; y < (yOffset + aoiHeight); y++) {
                                for (int x = xOffset; x < (xOffset + aoiWidth); x += bpp) {
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
                _outputImage.Value = _inputImage.Value;
            } catch(Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
            return true;
        }
    }
}

