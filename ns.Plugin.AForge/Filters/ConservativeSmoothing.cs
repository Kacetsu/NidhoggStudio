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
    public class ConservativeSmoothing : Tool {
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _kernelSize;

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "The filter implements conservative smoothing,\n"
                    + "which is a noise reduction technique that derives its name from the fact that it employs a simple,\n"
                    + "fast filtering algorithm that sacrifices noise suppression power in order to preserve the high spatial\n"
                    + "frequency detail (e.g. sharp edges) in an image. It is explicitly designed to remove noise spikes -\n"
                    + "isolated pixels of exceptionally low or high pixel intensity (salt and pepper noise).\n"
                    + "The filter accepts 8 bpp grayscale images and 24/32 bpp color images for processing.";
            }
        }

        public ConservativeSmoothing() {
            DisplayName = "AForge Conservative Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("KernelSize", 7, 3, 25));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _kernelSize = GetProperty("KernelSize") as IntegerProperty;
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

                AFilter.ConservativeSmoothing filter = new AFilter.ConservativeSmoothing();
                filter.KernelSize = (int)_kernelSize.Value;
                Bitmap destination = filter.Apply(source);

                _imageOutput.Value = Converter.ToImageContainer(destination as Bitmap);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}