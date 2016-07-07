using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    public class ImageStream : Node, IDisposable {

        public ImageStream() : base() {
        }

        public ImageStream(ImageContainer imageContainer) : this() {
            Width = imageContainer.Width;
            Height = imageContainer.Height;
            Stride = imageContainer.Stride;
            BytesPerPixel = imageContainer.BytesPerPixel;
            Stream = new BlockingStream(imageContainer.Data.Length);
            Stream.Write(imageContainer.Data, 0, imageContainer.Data.Length);
        }

        /// <summary>
        /// Gets the bytes per pixel.
        /// </summary>
        /// <value>
        /// The bytes per pixel.
        /// </value>
        public byte BytesPerPixel { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>
        /// The stream.
        /// </value>
        public BlockingStream Stream { get; private set; }

        /// <summary>
        /// Gets the stride.
        /// </summary>
        /// <value>
        /// The stride.
        /// </value>
        public int Stride { get; private set; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (Stream != null) Stream.Dispose();
            }
        }
    }
}