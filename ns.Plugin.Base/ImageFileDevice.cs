﻿using ns.Base.Attribute;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ns.Plugin.Base {

    [Visible, DataContract]
    public class ImageFileDevice : ImageDevice {
        private List<Bitmap> _bitmaps;
        private string _directory = string.Empty;
        private int _imageIndex = 0;
        private ImageProperty _imageProperty;
        private List<string> _openImageFilenames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileDevice"/> class.
        /// </summary>
        public ImageFileDevice() {
            DisplayName = "Image File Device";
            AddChild(new StringProperty("Directory", BaseManager.DocumentsPath + "Images"));
            AddChild(new ImageProperty("Image", true));
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
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            _openImageFilenames = new List<string>();
            _bitmaps = new List<Bitmap>();
            _directory = GetProperty<StringProperty>("Directory")?.Value;
            _imageProperty = GetProperty<ImageProperty>("Image");

            if (!Directory.Exists(_directory)) {
                ns.Base.Log.Trace.WriteLine("[" + Name + "] directory " + _directory + " does not exist, will create it now!", TraceEventType.Warning);
                Directory.CreateDirectory(_directory);
            }

            int imageCount = UpdateImageFiles();

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(_directory);
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileSystemWatcher.EnableRaisingEvents = true;

            if (imageCount == 0) {
                ns.Base.Log.Trace.WriteLine("No images found in " + _directory, TraceEventType.Warning);
            }

            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
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
                    BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                    stride = data.Stride;
                    Marshal.Copy(data.Scan0, byteArray, 0, size);
                    img.UnlockBits(data);
                }
                return byteArray;
            } catch (Exception ex) {
                ns.Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                stride = 0;
                return new byte[0];
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e) => UpdateImageFiles();

        private void OnRenamed(object source, RenamedEventArgs e) => UpdateImageFiles();

        private int UpdateImageFiles() {
            int imageCount = 0;

            string[] filenames = Directory.GetFiles(_directory);

            foreach (string filename in filenames) {
                if (filename.EndsWith(".bmp", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".jpg", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".png", true, System.Globalization.CultureInfo.CurrentCulture)) {
                    if (_openImageFilenames.Contains(filename)) continue;

                    Bitmap bitmap = new Bitmap(filename);
                    _bitmaps.Add(bitmap);
                    imageCount++;
                }
            }

            return imageCount;
        }
    }
}