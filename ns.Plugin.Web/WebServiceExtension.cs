using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using ns.Base.Attribute;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using ns.Plugin.Web.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ns.Plugin.Web {

    [Visible]
    public class WebServiceExtension : Extension {
        private int _port = 9999;

        private IHubContext _renderHubContext;
        private Thread _thread;
        private bool _isTerminated = false;

        private PropertyManager _propertyManager;
        private Queue<ImageProperty> _imageQueue;
        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address {
            get {
                return "http://" + "localhost" + ":" + _port.ToString();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServiceExtension"/> class.
        /// </summary>
        public WebServiceExtension() { }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            WebApp.Start<OwinStartup>(Address);
            _renderHubContext = GlobalHost.ConnectionManager.GetHubContext<RenderHub>();

            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            _imageQueue = new Queue<ImageProperty>();

            _thread = new Thread(new ThreadStart(() => this.ThreadLoop()));
            _thread.Start();

            return true;
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            _isTerminated = true;
            if (_thread == null) return true;

            while (!_thread.Join(0)) Thread.Sleep(1);
            _isTerminated = false;

            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            try {
                ImageProperty property = _propertyManager.Nodes.FindLast(i => i is ImageProperty) as ImageProperty;
                if (_imageQueue.Count > 2)
                    _imageQueue.Dequeue();
                _imageQueue.Enqueue(property.Clone() as ImageProperty);
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Threads the loop.
        /// </summary>
        private void ThreadLoop() {
            _stopwatch.Start();
            while (!_isTerminated) {
                if (_imageQueue.Count > 0 && _stopwatch.ElapsedMilliseconds > 50) {
                    byte[] imageData = ConvertToByteArry(_imageQueue.Dequeue());
                    if (imageData == null)
                        continue;
                    _renderHubContext.Clients.All.newImage(imageData);
                    _stopwatch.Restart();
                }

                Thread.Sleep(1);
            }
            _stopwatch.Stop();
        }

        /// <summary>
        /// Converts to byte arry.
        /// </summary>
        /// <param name="imageProperty">The image property.</param>
        /// <returns></returns>
        private byte[] ConvertToByteArry(ImageProperty imageProperty) {
            if (imageProperty == null || imageProperty.Value.Data == null || imageProperty.Value.Data.Count() < 1) return null;

            ImageContainer container = imageProperty.Value;
            PixelFormat pixelFormat = PixelFormat.Format24bppRgb;

            EncoderParameters codecParams;

            if (container.BytesPerPixel == 1) {
                pixelFormat = PixelFormat.Format8bppIndexed;
                EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, 30L);
                EncoderParameter encoderParameter2 = new EncoderParameter(Encoder.ColorDepth, 8L);
                codecParams = new EncoderParameters(2);
                codecParams.Param[0] = encoderParameter;
                codecParams.Param[1] = encoderParameter2;
            } else {
                EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, 30L);
                codecParams = new EncoderParameters(1);
                codecParams.Param[0] = encoderParameter;
            }

            Bitmap bitmap = new Bitmap(container.Width, container.Height, pixelFormat);

            if (container.BytesPerPixel == 1) {
                ColorPalette palette = bitmap.Palette;
                for (int index = 0; index < 256; index++) {
                    Color color = Color.FromArgb(index, index, index);
                    palette.Entries[index] = color;
                }
                bitmap.Palette = palette;
            }

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, container.Width, container.Height), ImageLockMode.ReadWrite, pixelFormat);
            Marshal.Copy(container.Data, 0, data.Scan0, container.Data.Length);
            bitmap.UnlockBits(data);

            ImageCodecInfo[] availableCodecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegCodecInfo = availableCodecs.FirstOrDefault(codec => codec.MimeType == "image/jpeg");

            if (jpegCodecInfo == null) {
                Base.Log.Trace.WriteLine("No available jpeg codec found!", TraceEventType.Error);
                return null;
            }

            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, jpegCodecInfo, codecParams);
                return stream.ToArray();
            }
        }
    }
}