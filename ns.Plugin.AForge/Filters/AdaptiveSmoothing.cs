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
    public class AdaptiveSmoothing : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        public AdaptiveSmoothing() {
            DisplayName = "AForge Adaptive Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "Adaptive Smoothing - noise removal with edges preserving.\n"
                    + "The filter accepts 8 bpp grayscale images and 24 bpp color images for processing.";
            }
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
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

                AFilter.AdaptiveSmoothing filter = new AFilter.AdaptiveSmoothing();

                _imageOutput.Value = Converter.ToImageContainer(filter.Apply(source));
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }

            return true;
        }
    }
}