using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class RectangleProperty : Property {
        public RectangleProperty() : base() { }
        public RectangleProperty(string name, bool isOutput) : base(name, isOutput) { }
        public RectangleProperty(string name, double x, double y, double width, double height) : base() {
            Name = name;
            List<double> values = new List<double>();
            values.Add(x);
            values.Add(y);
            values.Add(width);
            values.Add(height);
            Value = values;
        }

        /// <summary>
        /// Gets or sets x offset.
        /// </summary>
        public double X {
            get { return ((List<double>)Value)[0]; }
            set {
                ((List<double>)Value)[0] = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or sets y offset.
        /// </summary>
        public double Y {
            get { return ((List<double>)Value)[1]; }
            set {
                ((List<double>)Value)[1] = value;
                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Gets or sets width.
        /// </summary>
        public double Width {
            get { return ((List<double>)Value)[2]; }
            set {
                ((List<double>)Value)[2] = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Gets or sets height.
        /// </summary>
        public double Height {
            get { return ((List<double>)Value)[3]; }
            set {
                ((List<double>)Value)[3] = value;
                OnPropertyChanged("Height");
            }
        }

        public override Type Type {
            get {
                return typeof(List<double>);
            }
        }

    }
}
