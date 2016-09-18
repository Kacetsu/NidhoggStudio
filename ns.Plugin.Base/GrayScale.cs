using ns.Base;
using ns.Base.Extensions;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    /// <summary>
    /// Converts a RGB image into grayscale 8 bit.
    /// </summary>
    [Visible, DataContract]
    public sealed class Grayscale : Tool {
        private ImageProperty _inputImage;
        private ImageProperty _outputImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        public Grayscale() {
            AddChild(new ImageProperty("InputImage", false));
            AddChild(new ImageProperty("OuputImage", true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Grayscale(Grayscale other) : base(other) {
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public override string Category {
            get {
                return ToolCategory.Common.GetDescription();
            }
        }

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description {
            get {
                return "Converts a RGB image into grayscale 8 bit.";
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new Grayscale(this);

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            base.Initialize();

            _inputImage = GetProperty<ImageProperty>("InputImage");
            _outputImage = GetProperty<ImageProperty>("OuputImage");
            return true;
        }

        /// <summary>
        /// Converts a RGB image into grayscale 8 bit.
        /// </summary>
        /// <returns></returns>
        public override bool TryRun() {
            ImageContainer inputContainer = _inputImage.Value;
            byte[] data = inputContainer.Data;
            byte bpp = inputContainer.BytesPerPixel;

            int width = inputContainer.Width;
            int height = inputContainer.Height;

            byte[] destination = new byte[width * height];

            if (bpp == 1) {
                _outputImage.Value = _inputImage.Value;
                return true;
            }

            unsafe
            {
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