using ns.Base.Attribute;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {

    [Visible, Serializable]
    public class Grayscale : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private DoubleProperty _redCoefficient;
        private DoubleProperty _greenCoefficient;
        private DoubleProperty _blueCoefficient;

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "This class is the base class for image grayscaling.\n"
                    + "Other classes should inherit from this class and specify RGB coefficients used for color image conversion to grayscale.\n"
                    + "The filter accepts 24, 32, 48 and 64 bpp color images and produces 8 (if source is 24 or 32 bpp image)\n"
                    + "or 16 (if source is 48 or 64 bpp image) bpp grayscale image.";
            }
        }

        public Grayscale() {
            DisplayName = "AForge Grayscale";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new DoubleProperty("RedCoefficient", 0.333, 0, 1));
            AddChild(new DoubleProperty("GreenCoefficient", 0.333, 0, 1));
            AddChild(new DoubleProperty("BlueCoefficient", 0.333, 0, 1));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _redCoefficient = GetProperty("RedCoefficient") as DoubleProperty;
            _greenCoefficient = GetProperty("GreenCoefficient") as DoubleProperty;
            _blueCoefficient = GetProperty("BlueCoefficient") as DoubleProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {
            try {
                ImageContainer inputContainer = _imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                AFilter.Grayscale filter = new AFilter.Grayscale(_redCoefficient.Value, _greenCoefficient.Value, _blueCoefficient.Value);
                Bitmap destination = filter.Apply(source);

                _imageOutput.Value = Converter.ToImageContainer(destination);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}