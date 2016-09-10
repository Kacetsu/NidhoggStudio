using System;
using System.Runtime.Serialization;

namespace ns.Base {

    [Serializable, DataContract]
    public class Rectangle {

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

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        [DataMember]
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        [DataMember]
        public double Width { get; set; }

        /// <summary>
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        [DataMember]
        public double Y { get; set; }
    }
}