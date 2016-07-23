using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Derived class to filter for matching devices.
    /// </summary>
    [DataContract]
    public abstract class ImageDevice : Device {

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDevice"/> class.
        /// </summary>
        public ImageDevice() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDevice"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ImageDevice(ImageDevice other) : base(other) { }
    }
}