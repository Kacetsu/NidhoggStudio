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
    public class BilateralSmoothing : Tool {
        private IntegerProperty _colorFactor;
        private DoubleProperty _colorPower;
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _kernelSize;
        private IntegerProperty _spatialFactor;

        public BilateralSmoothing() {
            DisplayName = "AForge Bilateral Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("KernelSize", 7));
            AddChild(new IntegerProperty("SpatialFactor", 10));
            AddChild(new IntegerProperty("ColorFactor", 60));
            AddChild(new DoubleProperty("ColorPower", 0.5));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "Bilateral filter conducts 'selective' Gaussian smoothing of areas of same color (domains)\n"
                    + "which removes noise and contrast artifacts while preserving sharp edges."
                    + "The filter accepts 8 bpp grayscale images and 24/32 bpp color images for processing";
            }
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _kernelSize = GetProperty<IntegerProperty>("KernelSize");
            _spatialFactor = GetProperty<IntegerProperty>("SpatialFactor");
            _colorFactor = GetProperty<IntegerProperty>("ColorFactor");
            _colorPower = GetProperty<DoubleProperty>("ColorPower");
            _imageOutput = GetProperty<ImageProperty>("ImageOutput");
            return true;
        }

        public override bool Run() {
            try {
                ImageContainer inputContainer = _imageInput.Value;
                int kernelSize = _kernelSize.Value;
                int spatialFactor = _spatialFactor.Value;
                int colorFactor = _colorFactor.Value;
                double colorPower = _colorPower.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.UnmanagedImage uSource = global::AForge.Imaging.UnmanagedImage.FromManagedImage(source);

                AFilter.BilateralSmoothing filter = new AFilter.BilateralSmoothing();
                filter.EnableParallelProcessing = true;
                filter.KernelSize = kernelSize;
                filter.SpatialFactor = spatialFactor;
                filter.ColorFactor = colorFactor;
                filter.ColorPower = colorPower;

                _imageOutput.Value = Converter.ToImageContainer(filter.Apply(source));
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}