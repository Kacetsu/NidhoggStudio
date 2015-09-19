using AForge.Video.DirectShow;
using ns.Base.Attribute;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace ns.Plugin.AForge {
    [Visible, Serializable]
    public class DirectShowDevice : ImageDevice {
        private ImageProperty _imageProperty;
        private ListProperty _deviceListProperty;
        private ListProperty _resolutionListProperty;
        private bool _isTerminated = true;

        [NonSerialized]
        VideoCaptureDevice _videoDevice = null;

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
                    Trace.WriteLine(device.Name, LogCategory.Debug);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }

        private void DeviceListProperty_PropertyChangedEvent(object sender, Base.Event.NodeChangedEventArgs e) {
            if (sender == _deviceListProperty && e.Name == "Value") {
                Trace.WriteLine(MethodInfo.GetCurrentMethod().Name + " not implemented!", LogCategory.Warning);
            }
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            try {
                _imageProperty = GetProperty("Image") as ImageProperty;
                _deviceListProperty = GetProperty("Selected") as ListProperty;
                _resolutionListProperty = GetProperty("Resolution") as ListProperty;
                
                _resolutionListProperty.PropertyChanged += ResolutionListProperty_PropertyChanged;

                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                VideoCapabilities[] videoCapabilities;

                foreach (FilterInfo device in videoDevices) {
                    if (device.Name == _deviceListProperty.Value as string) {
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        private void ResolutionListProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Value" && !_isTerminated) {
                _videoDevice.Stop();
                _videoDevice.VideoResolution = _videoDevice.VideoCapabilities[_resolutionListProperty.Index];
                _videoDevice.Start();
            }
        }

        private bool _imageAcquired = false;

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {

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

        /// <summary>
        /// Finalize the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            _videoDevice.SignalToStop();
            _videoDevice.WaitForStop();
            base.Finalize();
            _isTerminated = true;
            return true;
        }

        private Bitmap _bitmap;

        private void _videoDevice_NewFrame(object sender, global::AForge.Video.NewFrameEventArgs eventArgs) {
            _bitmap = eventArgs.Frame.Clone() as Bitmap;    
            _imageAcquired = true;
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
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img.PixelFormat);
            stride = data.Stride;
            Marshal.Copy(data.Scan0, byteArray, 0, size);
            img.UnlockBits(data);

#if STOPWATCH
            stopwatch.Stop();
            Trace.WriteLine("Stopwatch: ImageToByteArray: " + stopwatch.ElapsedMilliseconds.ToString(), LogCategory.Debug);
#endif
            return byteArray;
        }
    }
}
