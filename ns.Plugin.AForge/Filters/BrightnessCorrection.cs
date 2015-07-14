using ns.Base.Attribute;
using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {
    [Visible, Serializable]
    public class BrightnessCorrection : Tool {

        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _adjustValue;

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "The filter operates in RGB color space and adjusts pixels' brightness by increasing every pixel's RGB values by the specified adjust value.\n" 
                    + "The filter accepts 8 bpp grayscale and 24/32 bpp color images for processing.";
            }
        }

        public BrightnessCorrection() {
            DisplayName = "AForge Brightness Correction";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("AdjustValue", 0, -255, 255));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _adjustValue = GetProperty("AdjustValue") as IntegerProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.Filters.BrightnessCorrection filter = new AFilter.BrightnessCorrection((int)_adjustValue.Value);
                Bitmap destination = filter.Apply(source);
                
                _imageOutput.Value = ns.Plugin.AForge.Converter.ToImageContainer(destination.Clone() as Bitmap);
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
