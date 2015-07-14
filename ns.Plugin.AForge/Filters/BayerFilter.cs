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
    public class BayerFilter : Tool {

        private ImageProperty _imageInput;
        private ImageProperty _imageOutput;

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

        public BayerFilter() {
            DisplayName = "AForge Bayer Filter";
            AddChild(new ImageProperty("ImageInput", false));
            AddChild(new ImageProperty("ImageOutput", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _imageInput = GetProperty("ImageInput") as ImageProperty;
            _imageOutput = GetProperty("ImageOutput") as ImageProperty;
            return true;
        }

        public override bool Run() {

            try {
                ImageContainer inputContainer = (ImageContainer)_imageInput.Value;

                PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

                if (inputContainer.BytesPerPixel == 1)
                    pixelFormat = PixelFormat.Format8bppIndexed;

                Bitmap source = ns.Plugin.AForge.Converter.ToBitmap(inputContainer.Data, inputContainer.Width, inputContainer.Height, inputContainer.Stride, pixelFormat);

                global::AForge.Imaging.Filters.BayerFilterOptimized filter = new AFilter.BayerFilterOptimized();
                Bitmap destination = filter.Apply(source);

                ImageContainer outputContainer = ns.Plugin.AForge.Converter.ToImageContainer(destination);
                
                _imageOutput.Value = outputContainer;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return true;
        }
    }
}
