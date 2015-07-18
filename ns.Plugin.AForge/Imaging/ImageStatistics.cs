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

namespace ns.Plugin.AForge.Imaging {
    [Visible, Serializable]
    public class ImageStatistics : Tool {

        private ImageProperty _imageInput;

        private IntegerProperty _maxBlue;
        private IntegerProperty _minBlue;
        private IntegerProperty _medianBlue;
        private DoubleProperty _meanBlue;
        private DoubleProperty _stdDevBlue;

        private IntegerProperty _maxGreen;
        private IntegerProperty _minGreen;
        private IntegerProperty _medianGreen;
        private DoubleProperty _meanGreen;
        private DoubleProperty _stdDevGreen;

        private IntegerProperty _maxRed;
        private IntegerProperty _minRed;
        private IntegerProperty _medianRed;
        private DoubleProperty _meanRed;
        private DoubleProperty _stdDevRed;

        public override string Category {
            get {
                return "AForge Imaging";
            }
        }

        public override string Description {
            get {
                return "The class is used to accumulate statistical values about images,\n" 
                    + "like histogram, mean, standard deviation, etc. for each color channel in RGB color space.\n"
                    + "The class accepts 8 bpp grayscale and 24/32 bpp color images for processing.";
            }
        }

        public ImageStatistics() {
            DisplayName = "AForge Image Statistics";
            AddChild(new ImageProperty("ImageInput", false));

            AddChild(new IntegerProperty("MaxBlue", true));
            AddChild(new IntegerProperty("MinBlue", true));
            AddChild(new IntegerProperty("MedianBlue", true));
            AddChild(new DoubleProperty("MeanBlue", true));
            AddChild(new DoubleProperty("StdDevBlue", true));

            AddChild(new IntegerProperty("MaxGreen", true));
            AddChild(new IntegerProperty("MinGreen", true));
            AddChild(new IntegerProperty("MedianGreen", true));
            AddChild(new DoubleProperty("MeanGreen", true));
            AddChild(new DoubleProperty("StdDevGreen", true));

            AddChild(new IntegerProperty("MaxRed", true));
            AddChild(new IntegerProperty("MinRed", true));
            AddChild(new IntegerProperty("MedianRed", true));
            AddChild(new DoubleProperty("MeanRed", true));
            AddChild(new DoubleProperty("StdDevRed", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;

            _maxBlue = GetProperty("MaxBlue") as IntegerProperty;
            _minBlue = GetProperty("MinBlue") as IntegerProperty;
            _medianBlue = GetProperty("MedianBlue") as IntegerProperty;
            _meanBlue = GetProperty("MeanBlue") as DoubleProperty;
            _stdDevBlue = GetProperty("StdDevBlue") as DoubleProperty;

            _maxGreen = GetProperty("MaxGreen") as IntegerProperty;
            _minGreen = GetProperty("MinGreen") as IntegerProperty;
            _medianGreen = GetProperty("MedianGreen") as IntegerProperty;
            _meanGreen = GetProperty("MeanGreen") as DoubleProperty;
            _stdDevGreen = GetProperty("StdDevGreen") as DoubleProperty;

            _maxRed = GetProperty("MaxRed") as IntegerProperty;
            _minRed = GetProperty("MinRed") as IntegerProperty;
            _medianRed = GetProperty("MedianRed") as IntegerProperty;
            _meanRed = GetProperty("MeanRed") as DoubleProperty;
            _stdDevRed = GetProperty("StdDevRed") as DoubleProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.ImageStatistics statistics = new global::AForge.Imaging.ImageStatistics(source);
                
                global::AForge.Math.Histogram blueHistogram = statistics.Blue;
                _maxBlue.Value = blueHistogram.Max;
                _minBlue.Value = blueHistogram.Min;
                _medianBlue.Value = blueHistogram.Median;
                _meanBlue.Value = blueHistogram.Mean;
                _stdDevBlue.Value = blueHistogram.StdDev;
                
                global::AForge.Math.Histogram greenHistogram = statistics.Green;
                _maxGreen.Value = greenHistogram.Max;
                _minGreen.Value = greenHistogram.Min;
                _medianGreen.Value = greenHistogram.Median;
                _meanGreen.Value = greenHistogram.Mean;
                _stdDevGreen.Value = greenHistogram.StdDev;

                global::AForge.Math.Histogram redHistogram = statistics.Red;
                _maxRed.Value = redHistogram.Max;
                _minRed.Value = redHistogram.Min;
                _medianRed.Value = redHistogram.Median;
                _meanRed.Value = redHistogram.Mean;
                _stdDevRed.Value = redHistogram.StdDev;

            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
