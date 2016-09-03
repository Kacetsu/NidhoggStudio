using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    /// <summary>
    /// Data structur containing image informations.
    /// </summary>
    [Serializable, DataContract]
    public struct ImageContainer : IEquatable<ImageContainer> {

        /// <summary>
        /// The bytes per pixel
        /// </summary>
        [DataMember]
        public byte BytesPerPixel;

        /// <summary>
        /// The data
        /// </summary>
        [DataMember]
        public byte[] Data;

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
        /// The width
        /// </summary>
        [DataMember]
        public int Width;

        public static bool operator !=(ImageContainer container1, ImageContainer container2) => !container1.Equals(container2);

        public static bool operator ==(ImageContainer container1, ImageContainer container2) => container1.Equals(container2);

        public bool Equals(ImageContainer other) {
            if (other == null) return false;

            return BytesPerPixel == other.BytesPerPixel && Data == other.Data && Height == other.Height && Width == other.Width && Stride == other.Stride;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            if (!(obj is ImageContainer)) return false;

            return Equals((ImageContainer)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

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