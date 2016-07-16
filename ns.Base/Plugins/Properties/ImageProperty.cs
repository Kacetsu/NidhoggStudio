using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class ImageProperty : GenericProperty<ImageContainer> {
        private bool _isVisible = false;

        public ImageProperty() : base() {
            CanAutoConnect = true;
        }

        public ImageProperty(string name, ImageContainer value) : base(name, value) {
            CanAutoConnect = true;
        }

        public ImageProperty(string name, bool isOutput) : base(name, isOutput) {
            CanAutoConnect = true;
        }

        /// <summary>
        /// Gets the bytes per pixel.
        /// </summary>
        /// <value>
        /// The bytes per pixel.
        /// </value>
        public byte BytesPerPixel => Value.BytesPerPixel;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height => Value.Height;

        public bool IsVisible {
            get { return _isVisible; }
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width => Value.Width;

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        public void SetValue(byte[] data, int width, int height, int stride, byte bytesPerPixel) {
            ImageContainer container = new ImageContainer();
            container.Data = data;
            container.Width = width;
            container.Height = height;
            container.Stride = stride;
            container.BytesPerPixel = bytesPerPixel;
            Value = container;
        }
    }
}