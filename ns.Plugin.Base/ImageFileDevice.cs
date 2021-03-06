﻿using ns.Base.Attribute;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ns.Plugin.Base {
    [Visible, Serializable]
    public class ImageFileDevice : ImageDevice {
        private string _directory = string.Empty;
        private ImageProperty _imageProperty;
        private List<Bitmap> _bitmaps;
        private int _imageIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileDevice"/> class.
        /// </summary>
        public ImageFileDevice() {
            DisplayName = "Image File Device";
            AddChild(new StringProperty("Directory", BaseManager.DocumentsPath + "Images"));
            AddChild(new ImageProperty("Image", true));
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            _bitmaps = new List<Bitmap>();
            _directory = GetProperty("Directory").Value as string;
            _imageProperty = GetProperty("Image") as ImageProperty;

            if (!Directory.Exists(_directory)) {
                Trace.WriteLine("[" + this.Name + "] directory " + _directory + " does not exist!", LogCategory.Error);
                return false;
            }
            string[] filenames = Directory.GetFiles(_directory);
            List<string> imageFiles = new List<string>();
            int imageCount = 0;

            foreach (string filename in filenames) {
                if (filename.EndsWith(".bmp", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".jpg", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".png", true, System.Globalization.CultureInfo.CurrentCulture)) {
                    imageFiles.Add(filename);
                    Bitmap bitmap = new Bitmap(filename);
                    _bitmaps.Add(bitmap);
                    imageCount++;
                }
            }

            if (imageCount == 0) {
                Trace.WriteLine("No images found in " + _directory, LogCategory.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Finalize() {
            if (_bitmaps != null) {
                foreach (Bitmap bitmap in _bitmaps)
                    bitmap.Dispose();
                _bitmaps.Clear();
            }
            _bitmaps = null;
            _imageIndex = 0;

            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            if (_imageIndex >= _bitmaps.Count)
                _imageIndex = 0;
            Bitmap bitmap = _bitmaps[_imageIndex];

            int bpp = 1;

            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed) {
                bpp = (bitmap.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            }

            int stride = 0;
            byte[] data = ImageToByteArray(bitmap, bpp, out stride);
            _imageProperty.SetValue(data, bitmap.Width, bitmap.Height, stride, (byte)bpp);
            _imageIndex++;

            return true;
        }

        /// <summary>
        /// Images to byte array.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <returns></returns>
        private byte[] ImageToByteArray(Bitmap img, int bpp, out int stride) {
            try {
                int size = img.Width * img.Height * bpp;
                byte[] byteArray = new byte[size];
                lock (img) {
                    BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img.PixelFormat);
                    stride = data.Stride;
                    Marshal.Copy(data.Scan0, byteArray, 0, size);
                    img.UnlockBits(data);
                }
                return byteArray;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                stride = 0;
                return new byte[0];
            }
        }
    }
}
