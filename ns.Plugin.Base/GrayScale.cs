﻿using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    /// <summary>
    /// Converts a RGB image into grayscale 8 bit.
    /// </summary>
    [Visible, DataContract]
    public sealed class Grayscale : Tool {

        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        public Grayscale() : base() {
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
        public override string Category => "Common";

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public override string Description => "Converts a RGB image into grayscale 8 bit.";

        /// <summary>
        /// Gets the input image.
        /// </summary>
        /// <value>
        /// The input image.
        /// </value>
        public ImageProperty InputImage => FindOrAdd<ImageProperty, ImageContainer>(new ImageContainer());

        /// <summary>
        /// Gets the output image.
        /// </summary>
        /// <value>
        /// The output image.
        /// </value>
        public ImageProperty OutputImage => FindOrAdd<ImageProperty, ImageContainer>(new ImageContainer(), PropertyDirection.Out);

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new Grayscale(this);

        /// <summary>
        /// Converts a RGB image into grayscale 8 bit.
        /// </summary>
        /// <returns></returns>
        public override bool TryRun() {
            ImageContainer inputContainer = InputImage.Value;
            byte[] data = inputContainer.Data;
            byte bpp = inputContainer.BytesPerPixel;

            int width = inputContainer.Width;
            int height = inputContainer.Height;

            byte[] destination = new byte[width * height];

            if (bpp == 1) {
                OutputImage.Value = InputImage.Value;
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

            OutputImage.Value = outContainer;
            return true;
        }
    }
}