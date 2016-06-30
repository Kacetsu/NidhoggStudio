﻿using ns.Base.Attribute;
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
    public class BlurFilter : Tool {
        private IntegerProperty _divisor;
        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;
        private IntegerProperty _threshold;

        public BlurFilter() {
            DisplayName = "AForge Blur Filter";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new IntegerProperty("Divisor", 50, int.MinValue, int.MaxValue));
            AddChild(new IntegerProperty("Threshold", 0));
            AddChild(new ImageProperty("ImageOutput", true));
        }

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

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty<ImageProperty>("ImageInput");
            _divisor = GetProperty<IntegerProperty>("Divisor");
            _threshold = GetProperty<IntegerProperty>("Threshold");
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

                AFilter.Blur filter = new AFilter.Blur();
                filter.Divisor = _divisor.Value;
                filter.Threshold = _threshold.Value;
                Bitmap destination = filter.Apply(source);

                _imageOutput.Value = Converter.ToImageContainer(destination as Bitmap);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return true;
        }
    }
}