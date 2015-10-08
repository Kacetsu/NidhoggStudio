using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Extensions;
using System.Xml.Serialization;
using System.Xml;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class NumberProperty<T> : Property {
        private object _max;
        private object _min;
        private Tolerance<T> _tolerance;

        public NumberProperty() : base() {
            _tolerance = new Tolerance<T>();
        }
        public NumberProperty(string name, T value) : base(name, value) { }
        public NumberProperty(string name, bool isOutput) : base(name, isOutput) { }
        public NumberProperty(string name, T value, T min, T max) : base(name, value) {
            _max = max;
            _min = min;
            _tolerance = new Tolerance<T>(min, max);
        }

        /// <summary>
        /// Gets if the Property has a numeric value.
        /// </summary>
        public override bool IsNumeric {
            get { return true; }
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object Max {
            get { return _max; }
            set { _max = value; }
        }

        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public object Min {
            get { return _min; }
            set { _min = value; }
        }

        public override bool IsToleranceDisabled {
            get { return _tolerance == null; }
        }

        public new Tolerance<T> Tolerance {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        public override object Clone() {
            NumberProperty<T> clone = this.DeepClone();
            return clone;
        }

        public override void Save(XmlWriter writer) {
            XmlSerializer ser = new XmlSerializer(typeof(Tolerance<T>));
            ser.Serialize(writer, this.Tolerance);
        }
    }
}
