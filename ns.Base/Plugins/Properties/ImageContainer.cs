using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    /// <summary>
    /// Data structur containing image informations.
    /// </summary>
    [Serializable]
    public struct ImageContainer {
        /// <summary>
        /// The data
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// The width
        /// </summary>
        public int Width;

        /// <summary>
        /// The height
        /// </summary>
        public int Height;

        /// <summary>
        /// The stride
        /// </summary>
        public int Stride;

        /// <summary>
        /// The bytes per pixel
        /// </summary>
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
