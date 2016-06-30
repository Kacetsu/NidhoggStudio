using ns.Base.Attribute;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;

namespace ns.Plugin.AForge.Imaging {

    [Visible, DataContract]
    public class ImageStatistics : Tool {
        private ImageProperty _imageInput;

        private IntegerProperty _maxBlue;
        private IntegerProperty _maxGreen;
        private IntegerProperty _maxRed;
        private DoubleProperty _meanBlue;
        private DoubleProperty _meanGreen;
        private DoubleProperty _meanRed;
        private IntegerProperty _medianBlue;
        private IntegerProperty _medianGreen;
        private IntegerProperty _medianRed;
        private IntegerProperty _minBlue;
        private IntegerProperty _minGreen;
        private IntegerProperty _minRed;
        private DoubleProperty _stdDevBlue;
        private DoubleProperty _stdDevGreen;
        private DoubleProperty _stdDevRed;

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

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _imageInput.IsVisible = true;

            _maxBlue = GetProperty<IntegerProperty>("MaxBlue");
            _minBlue = GetProperty<IntegerProperty>("MinBlue");
            _medianBlue = GetProperty<IntegerProperty>("MedianBlue");
            _meanBlue = GetProperty<DoubleProperty>("MeanBlue");
            _stdDevBlue = GetProperty<DoubleProperty>("StdDevBlue");

            _maxGreen = GetProperty<IntegerProperty>("MaxGreen");
            _minGreen = GetProperty<IntegerProperty>("MinGreen");
            _medianGreen = GetProperty<IntegerProperty>("MedianGreen");
            _meanGreen = GetProperty<DoubleProperty>("MeanGreen");
            _stdDevGreen = GetProperty<DoubleProperty>("StdDevGreen");

            _maxRed = GetProperty<IntegerProperty>("MaxRed");
            _minRed = GetProperty<IntegerProperty>("MinRed");
            _medianRed = GetProperty<IntegerProperty>("MedianRed");
            _meanRed = GetProperty<DoubleProperty>("MeanRed");
            _stdDevRed = GetProperty<DoubleProperty>("StdDevRed");
            return true;
        }

        public override bool Run() {
            try {
                ImageContainer inputContainer = _imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

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
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}