using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class ListProperty : Property {

        private List<object> _list = new List<object>();

        public ListProperty() : base() { }

        public ListProperty(string name, List<object> values) : base(name, values) {
            _list = values;
            if (values.Count == 0)
                this.Value = "INVALID";
            else
                this.Value = values[0];
        }

        public ListProperty(string name,  bool isOutput)
            : base(name, isOutput) {
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public List<object> List {
            get { return _list; }
            set { _list = value; }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index {
            get {
                int index = 0;
                for (; index < _list.Count; index++) {
                    if (_list[index].ToString() == Value.ToString()) break;
                    else if (_list[index] == Value) break;
                }
                return index;
            }
        }

        public override Type Type {
            get {
                return typeof(List<object>);
            }
        }
    }
}
