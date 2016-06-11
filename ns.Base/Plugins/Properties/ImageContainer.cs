using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    /// <summary>
    /// Data structur containing image informations.
    /// </summary>
    [Serializable, DataContract]
    public struct ImageContainer {

        /// <summary>
        /// The data
        /// </summary>
        [DataMember]
        public byte[] Data;

        /// <summary>
        /// The width
        /// </summary>
        [DataMember]
        public int Width;

        /// <summary>
        /// The height
        /// </summary>
        [DataMember]
        public int Height;

        /// <summary>
        /// The stride
        /// </summary>
        [DataMember]
        public int Stride;

        /// <summary>
        /// The bytes per pixel
        /// </summary>
        [DataMember]
        public byte BytesPerPixel;

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() {
            return Width.ToString() + " x " + Height.ToString() + " : " + BytesPerPixel.ToString() + "bpp";
        }
    }
}