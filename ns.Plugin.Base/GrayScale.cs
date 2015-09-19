using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;

namespace ns.Plugin.Base {
    /// <summary>
    /// Converts a RGB image into grayscale 8 bit.
    /// </summary>
    [Visible, Serializable]
    public class Grayscale : Tool {
        private ImageProperty _inputImage;
        private ImageProperty _outputImage;

        public override string Category {
            get {
                return ToolCategory.Common.GetDescription();
            }
        }

        public override string Description {
            get {
                return "Converts a RGB image into grayscale 8 bit.";
            }
        }

        public Grayscale() {
            AddChild(new ImageProperty("InputImage", false));
            AddChild(new ImageProperty("OuputImage", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _inputImage = GetProperty("InputImage") as ImageProperty;
            _outputImage = GetProperty("OuputImage") as ImageProperty;
            return true;
        }

        /// <summary>
        /// Converts a RGB image into grayscale 8 bit.
        /// </summary>
        /// <returns></returns>
        public override bool Run() {
            ImageContainer inputContainer = (ImageContainer)_inputImage.Value;
            byte[] data = inputContainer.Data;
            byte bpp = inputContainer.BytesPerPixel;

            int width = inputContainer.Width;
            int height = inputContainer.Height;

            byte[] destination = new byte[width * height];

            if (bpp == 1) {
                _outputImage.Value = _inputImage.Value;
                return true;
            }

            unsafe {
                fixed (byte* ptr = data) {
                    fixed (byte* destPtr = destination) {
                        for (int y = 0; y < height; y++) {
                            for (int x = 0; x < width; x++) {
                                byte r = ptr[(y * width + x) * bpp];
                                byte g = ptr[(y * width + x) * bpp + 1];
                                byte b = ptr[(y * width + x) * bpp + 2];
                                destPtr[(y * width + x)] = (byte)((r + g + b) / 3);
                            }
                        }
                    }
                }
            }

            ImageContainer outContainer = new ImageContainer();
            outContainer.Data = destination;
            outContainer.BytesPerPixel = 1;
            outContainer.Height = height;
            outContainer.Width = width;
            outContainer.Stride = width;

            _outputImage.Value = outContainer;
            return true;
        }
    }
}
