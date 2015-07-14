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
    public class BilateralSmoothing : Tool {

        private ImageProperty _imageInput;
        private IntegerProperty _kernelSize;
        private IntegerProperty _spatialFactor;
        private IntegerProperty _colorFactor;
        private DoubleProperty _colorPower;
        private ImageProperty _imageOutput;

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

        public BilateralSmoothing() {
            DisplayName = "AForge Bilateral Smoothing";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("KernelSize", 7));
            AddChild(new IntegerProperty("SpatialFactor", 10));
            AddChild(new IntegerProperty("ColorFactor", 60));
            AddChild(new DoubleProperty("ColorPower", 0.5));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _kernelSize = GetProperty("KernelSize") as IntegerProperty;
            _spatialFactor = GetProperty("SpatialFactor") as IntegerProperty;
            _colorFactor = GetProperty("ColorFactor") as IntegerProperty;
            _colorPower = GetProperty("ColorPower") as DoubleProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;
                int kernelSize = (int)_kernelSize.Value;
                int spatialFactor = (int)_spatialFactor.Value;
                int colorFactor = (int)_colorFactor.Value;
                double colorPower = (double)_colorPower.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.UnmanagedImage uSource = global::AForge.Imaging.UnmanagedImage.FromManagedImage(source);

                global::AForge.Imaging.Filters.BilateralSmoothing filter = new AFilter.BilateralSmoothing();
                filter.EnableParallelProcessing = true;
                filter.KernelSize = kernelSize;
                filter.SpatialFactor = spatialFactor;
                filter.ColorFactor = colorFactor;
                filter.ColorPower = colorPower;

                _imageOutput.Value = ns.Plugin.AForge.Converter.ToImageContainer(filter.Apply(source));
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
