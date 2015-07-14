using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ns.Plugin.AForge {
    public static class Converter {

        /// <summary>
        /// To the bitmap.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <returns></returns>
        public static Bitmap ToBitmap(byte[] bytes, int width, int height, int stride, PixelFormat pixelFormat) {
            Bitmap bitmap = new Bitmap(width, height, pixelFormat);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, pixelFormat);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }

        /// <summary>
        /// To the image container.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        public static ImageContainer ToImageContainer(Bitmap bitmap) {
            int bpp = 3;

            switch (bitmap.PixelFormat) {
                case PixelFormat.Format32bppRgb:
                    bpp = 4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    bpp = 1;
                    break;
                default:
                    bpp = 3;
                    break;
            }


            int size = bitmap.Width * bitmap.Height * bpp;
            byte[] byteArray = new byte[size];
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int stride = data.Stride;
            Marshal.Copy(data.Scan0, byteArray, 0, size);
            bitmap.UnlockBits(data);

            ImageContainer container;
            container.BytesPerPixel = (byte)bpp;
            container.Data = byteArray;
            container.Height = bitmap.Height;
            container.Stride = stride;
            container.Width = bitmap.Width;

            return container;
        }
    }
}
