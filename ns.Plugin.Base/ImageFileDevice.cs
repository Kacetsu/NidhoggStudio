using ns.Base;
using ns.Base.Manager;
using ns.Base.Plugins.Devices;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace ns.Plugin.Base {

    [DataContract]
    public sealed class ImageFileDevice : ImageDevice {
        private List<Bitmap> _bitmaps;
        private int _imageIndex = 0;
        private List<string> _openImageFilenames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileDevice"/> class.
        /// </summary>
        public ImageFileDevice() : base() {
            DisplayName = "Image File Device";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileDevice"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ImageFileDevice(ImageFileDevice other) : base(other) { }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>
        /// The directory.
        /// </value>
        public StringProperty DirectoryPath => FindOrAdd<StringProperty, string>(string.Concat(BaseManager.DocumentsPath, "Images"));

        /// <summary>
        /// Gets the framerate.
        /// </summary>
        /// <value>
        /// The framerate.
        /// </value>
        public DoubleProperty Framerate => FindOrAdd<DoubleProperty, double>(30d);

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new ImageFileDevice(this);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            if (_bitmaps != null) {
                foreach (Bitmap bitmap in _bitmaps)
                    bitmap.Dispose();
                _bitmaps.Clear();
            }
            _bitmaps = null;
            _imageIndex = 0;

            base.Close();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override void Initialize() {
            base.Initialize();
            _openImageFilenames = new List<string>();
            _bitmaps = new List<Bitmap>();

            if (!Directory.Exists(DirectoryPath.Value)) {
                ns.Base.Log.Trace.WriteLine(string.Concat("[", Name, "] directory ", DirectoryPath.Value, " does not exist, will create it now!"), TraceEventType.Warning);
                Directory.CreateDirectory(DirectoryPath.Value);
            }

            int imageCount = UpdateImageFiles();

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(DirectoryPath.Value);
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);
            fileSystemWatcher.EnableRaisingEvents = true;

            if (imageCount == 0) {
                ns.Base.Log.Trace.WriteLine("No images found in " + DirectoryPath.Value, TraceEventType.Warning);
            }
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < (1000.0 / Framerate.Value)) {
                Thread.Sleep(1);
            }

            if (_imageIndex >= _bitmaps.Count)
                _imageIndex = 0;
            Bitmap bitmap = _bitmaps[_imageIndex];

            int bpp = 1;

            if (bitmap.PixelFormat != PixelFormat.Format8bppIndexed) {
                bpp = (bitmap.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            }

            int stride = 0;
            byte[] data = ImageToByteArray(bitmap, bpp, out stride);
            OutputImage.SetValue(data, bitmap.Width, bitmap.Height, stride, (byte)bpp);
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
                BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                stride = data.Stride;
                Marshal.Copy(data.Scan0, byteArray, 0, size);
                img.UnlockBits(data);
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

            string[] filenames = Directory.GetFiles(DirectoryPath.Value);

            foreach (string filename in filenames) {
                if (filename.EndsWith(".bmp", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".jpg", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".png", true, System.Globalization.CultureInfo.CurrentCulture)
                    || filename.EndsWith(".gif", true, System.Globalization.CultureInfo.CurrentCulture)) {
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