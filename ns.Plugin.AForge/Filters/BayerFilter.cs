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
    public class BayerFilter : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

        public BayerFilter() {
            DisplayName = "AForge Bayer Filter";
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
                return "The class implements Bayer filter routine, which creates color image\n"
                    + "out of grayscale image produced by image sensor built with Bayer color matrix.\n"
                    + "TThe filter accepts 8 bpp grayscale images and produces 24 bpp RGB image.";
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

                AFilter.BayerFilterOptimized filter = new AFilter.BayerFilterOptimized();
                Bitmap destination = filter.Apply(source);

                ImageContainer outputContainer = Converter.ToImageContainer(destination);

                _imageOutput.Value = outputContainer;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}