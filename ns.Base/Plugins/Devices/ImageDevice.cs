using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Devices {

    [DataContract]
    public abstract class ImageDevice : Device {

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDevice"/> class.
        /// </summary>
        public ImageDevice()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDevice"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ImageDevice(ImageDevice other)
            : base(other) {
        }

        /// <summary>
        /// Gets the output image.
        /// </summary>
        /// <value>
        /// The output image.
        /// </value>
        public ImageProperty OutputImage => FindOrAdd<ImageProperty, ImageContainer>(new ImageContainer(), PropertyDirection.Out);
    }
}