using System;
using System.Runtime.Serialization;

namespace ns.Base {

    [Serializable, DataContract]
    public class Rectangle {

        /// <summary>
        /// The x
        /// </summary>
        [DataMember]
        public double X;

        /// <summary>
        /// The y
        /// </summary>
        [DataMember]
        public double Y;

        /// <summary>
        /// The width
        /// </summary>
        [DataMember]
        public double Width;

        /// <summary>
        /// The height
        /// </summary>
        [DataMember]
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