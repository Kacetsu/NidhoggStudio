using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class NumberProperty : Property {
        private object _max;
        private object _min;

        public NumberProperty() : base() { }
        public NumberProperty(string name, object value) : base(name, value) { }
        public NumberProperty(string name, bool isOutput) : base(name, isOutput) { }
        public NumberProperty(string name, object value, object min, object max) : base(name, value) {
            _max = max;
            _min = min;
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

    }
}
