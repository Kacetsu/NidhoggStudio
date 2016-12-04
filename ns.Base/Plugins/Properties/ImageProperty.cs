using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class ImageProperty : GenericProperty<ImageContainer> {
        private bool _isVisible = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperty"/> class.
        /// </summary>
        public ImageProperty() : base() {
            CanAutoConnect = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ImageProperty(ImageProperty other) : base(other) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public ImageProperty(ImageContainer value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
            : base(value, direction, name) {
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

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible {
            get { return _isVisible; }
            set {
                if (_isVisible != value) {
                    _isVisible = value;
                    OnPropertyChanged();
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
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new ImageProperty(this);

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