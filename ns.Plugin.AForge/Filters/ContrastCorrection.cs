﻿using ns.Base.Attribute;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {
    [Visible, Serializable]
    public class ContrastCorrection : Tool {

        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _factor;

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
        /// Initializes a new instance of the <see cref="ContrastCorrection"/> class.
        /// </summary>
        public ContrastCorrection() {
            DisplayName = "AForge Contrast Correction";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("Factor", 0, -127, 127));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _factor = GetProperty("Factor") as IntegerProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.Filters.ContrastCorrection filter = new AFilter.ContrastCorrection((int)_factor.Value);
                Bitmap destination = filter.Apply(source);
                
                _imageOutput.Value = ns.Plugin.AForge.Converter.ToImageContainer(destination as Bitmap);
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
