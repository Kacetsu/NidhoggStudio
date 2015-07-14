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
    public class Add : Tool {

        private ImageProperty _imageInput;
        private ImageProperty _imageOverlay;
        private ImageProperty _imageOutput;

        public override string Category {
            get {
                return "AForge Filter";
            }
        }

        public override string Description {
            get {
                return "The add filter takes two images (source and overlay images)\n" 
                    + "of the same size and pixel format and produces an image,\n"
                    + "where each pixel equals to the sum value of corresponding pixels from provided images" 
                    + "(if sum is greater than maximum allowed value, 255 or 65535, then it is truncated to that maximum).\n"
                    + "The filter accepts 8 and 16 bpp grayscale images and 24, 32, 48 and 64 bpp color images for processing.";
            }
        }

        public Add() {
            DisplayName = "AForge Add";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOverlay", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _imageOverlay = GetProperty("ImageOverlay") as ImageProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;
                ImageContainer overlayContainer = (ImageContainer)_imageOverlay.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);
                Bitmap overlay = ns.Plugin.AForge.Converter.ToBitmap(overlayContainer.Data, overlayContainer.Width, overlayContainer.Height, overlayContainer.Stride, pixelFormat);

                global::AForge.Imaging.UnmanagedImage uSource = global::AForge.Imaging.UnmanagedImage.FromManagedImage(source);
                global::AForge.Imaging.UnmanagedImage uOverlay = global::AForge.Imaging.UnmanagedImage.FromManagedImage(overlay);

                global::AForge.Imaging.Filters.Add filter = new AFilter.Add(uOverlay);
                filter.ApplyInPlace(uSource);

                _imageOutput.Value = inputContainer;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
