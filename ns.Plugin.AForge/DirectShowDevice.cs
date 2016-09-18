using AForge.Video.DirectShow;
using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;

namespace ns.Plugin.AForge {

    [Visible, DataContract]
    public sealed class DirectShowDevice : ImageDevice {
        private Bitmap _bitmap;
        private ListProperty _deviceListProperty;
        private bool _imageAcquired = false;
        private ImageProperty _imageProperty;
        private bool _isTerminated = true;
        private ListProperty _resolutionListProperty;
        private VideoCaptureDevice _videoDevice = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectShowDevice"/> class.
        /// </summary>
        public DirectShowDevice() {
            try {
                DisplayName = "DirectShow Device";
                AddChild(new ImageProperty("Image", true));

                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                List<object> values = new List<object>();

                foreach (FilterInfo device in videoDevices) {
                    values.Add(device.Name);
                    Base.Log.Trace.WriteLine(device.Name, TraceEventType.Verbose);
                }

                VideoCapabilities[] videoCapabilities;

                if (videoDevices.Count > 0)
                    _videoDevice = new VideoCaptureDevice(videoDevices[0].MonikerString);

                AddChild(new ListProperty("Selected", values));

                List<object> resolutions = new List<object>();

                if (_videoDevice != null) {
                    videoCapabilities = _videoDevice.VideoCapabilities;
                    foreach (VideoCapabilities vc in videoCapabilities) {
                        resolutions.Add(vc.FrameSize.Width + " x " + vc.FrameSize.Height);
                    }
                }

                AddChild(new ListProperty("Resolution", resolutions));
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectShowDevice"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public DirectShowDevice(DirectShowDevice other) : base(other) {
        }

        public override Node Clone() => new DirectShowDevice(this);

        /// <summary>
        /// Finalize the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override void Close() {
            _videoDevice?.SignalToStop();
            _videoDevice?.WaitForStop();
            _isTerminated = true;
            base.Close();
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            try {
                _imageProperty = GetProperty<ImageProperty>("Image");
                _deviceListProperty = GetProperty<ListProperty>("Selected");
                _resolutionListProperty = GetProperty<ListProperty>("Resolution");

                _resolutionListProperty.PropertyChanged += ResolutionListProperty_PropertyChanged;

                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                VideoCapabilities[] videoCapabilities;

                foreach (FilterInfo device in videoDevices) {
                    if (device.Name == _deviceListProperty.SelectedItem as string) {
                        _videoDevice = new VideoCaptureDevice(device.MonikerString);
                        break;
                    }
                }

                if (_videoDevice == null) return false;

                videoCapabilities = _videoDevice.VideoCapabilities;
                _videoDevice.VideoResolution = videoCapabilities[_resolutionListProperty.Index];

                _videoDevice.NewFrame += _videoDevice_NewFrame;
                _videoDevice.Start();
                _isTerminated = false;
                return true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool TryRun() {
            while (!_imageAcquired && !_isTerminated)
                Thread.Sleep(1);
            if (_isTerminated)
                return true;

            int stride = 0;
            byte[] data = ImageToByteArray(_bitmap, out stride);
            _imageProperty.SetValue(data, _bitmap.Width, _bitmap.Height, stride, 3);

            _imageAcquired = false;
            return true;
        }

        private void _videoDevice_NewFrame(object sender, global::AForge.Video.NewFrameEventArgs eventArgs) {
            _bitmap = eventArgs.Frame.Clone() as Bitmap;
            _imageAcquired = true;
        }

        private void DeviceListProperty_PropertyChangedEvent(object sender, Base.Event.NodeChangedEventArgs e) {
            if (sender == _deviceListProperty && e.Name == "Value") {
                Base.Log.Trace.WriteLine(MethodBase.GetCurrentMethod().Name + " not implemented!", TraceEventType.Warning);
            }
        }

        private byte[] ImageToByteArray(Bitmap img, out int stride) {
#if STOPWATCH
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif
            int bpp = 3;
            if (img.PixelFormat == PixelFormat.Format32bppRgb)
                bpp = 4;

            int size = img.Width * img.Height * bpp;
            byte[] byteArray = new byte[size];
            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            stride = data.Stride;
            Marshal.Copy(data.Scan0, byteArray, 0, size);
            img.UnlockBits(data);

#if STOPWATCH
            stopwatch.Stop();
            Base.Log.Trace.WriteLine("Stopwatch: ImageToByteArray: " + stopwatch.ElapsedMilliseconds.ToString(), TraceEventType.Verbose);
#endif
            return byteArray;
        }

        private void ResolutionListProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Value" && !_isTerminated) {
                _videoDevice.Stop();
                _videoDevice.VideoResolution = _videoDevice.VideoCapabilities[_resolutionListProperty.Index];
                _videoDevice.Start();
            }
        }
    }
}