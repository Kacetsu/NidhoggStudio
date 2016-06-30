using ns.Base.Attribute;
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
    public class BrightnessCorrection : Tool {
        private IntegerProperty _adjustValue;
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        public BrightnessCorrection() {
            DisplayName = "AForge Brightness Correction";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("AdjustValue", 0, -255, 255));
            AddChild(new ImageProperty("ImageOutput", true));
        }

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

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _adjustValue = GetProperty<IntegerProperty>("AdjustValue");
            _imageOutput = GetProperty<ImageProperty>("ImageOutput");
            return true;
        }

        public override bool Run() {
            try {
                ImageContainer inputContainer = _imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                AFilter.BrightnessCorrection filter = new AFilter.BrightnessCorrection((int)_adjustValue.Value);
                Bitmap destination = filter.Apply(source);

                _imageOutput.Value = Converter.ToImageContainer(destination.Clone() as Bitmap);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}