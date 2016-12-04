using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class RectangleProperty : GenericProperty<Rectangle> {

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty"/> class.
        /// </summary>
        public RectangleProperty()
            : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public RectangleProperty(RectangleProperty other)
            : base(other) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleProperty" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="">The .</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        public RectangleProperty(Rectangle value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null)
                    : base(value, direction, name) {
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
        public override Type Type => typeof(Rectangle);

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
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public override Node Clone() => new RectangleProperty(this);
    }
}