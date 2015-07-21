using ns.Base.Attribute;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using AFilter = global::AForge.Imaging.Filters;

namespace ns.Plugin.AForge.Filters {
    [Visible, Serializable]
    public class BlurFilter : Tool {

        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _divisor;
        private IntegerProperty _threshold;

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "";
            }
        }

        public BlurFilter() {
            DisplayName = "AForge Blur Filter";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("Divisor", 50, int.MinValue, int.MaxValue));
            AddChild(new IntegerProperty("Threshold", 0));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _divisor = GetProperty("Divisor") as IntegerProperty;
            _threshold = GetProperty("Threshold") as IntegerProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                AFilter.Blur filter = new AFilter.Blur();
                filter.Divisor = (int)_divisor.Value;
                filter.Threshold = (int)_threshold.Value;
                Bitmap destination = filter.Apply(source);
                
                _imageOutput.Value = Converter.ToImageContainer(destination as Bitmap);
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
