using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class RectangleProperty : GenericProperty<Rectangle> {

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty"/> class.
        /// </summary>
        public RectangleProperty() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public RectangleProperty(string name, bool isOutput) : base(name, isOutput) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleProperty(string name, double x, double y, double width, double height) : base() {
            Name = name;
            Value = new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Gets or sets x offset.
        /// </summary>
        [XmlIgnore]
        public double X {
            get {
                return Value.X;
            }
            set {
                Value.X = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets y offset.
        /// </summary>
        [XmlIgnore]
        public double Y {
            get {
                return Value.Y;
            }
            set {
                Value.Y = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets width.
        /// </summary>
        [XmlIgnore]
        public double Width {
            get {
                return Value.Width;
            }
            set {
                Value.Width = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets height.
        /// </summary>
        [XmlIgnore]
        public double Height {
            get {
                return Value.Height;
            }
            set {
                Value.Height = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type {
            get {
                return typeof(Rectangle);
            }
        }
    }
}