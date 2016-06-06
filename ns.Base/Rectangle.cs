using System;

namespace ns.Base {

    [Serializable]
    public class Rectangle {

        /// <summary>
        /// The x
        /// </summary>
        public double X;

        /// <summary>
        /// The y
        /// </summary>
        public double Y;

        /// <summary>
        /// The width
        /// </summary>
        public double Width;

        /// <summary>
        /// The height
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        public Rectangle() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rectangle(double x, double y, double width, double height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}