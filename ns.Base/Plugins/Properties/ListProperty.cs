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

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        public ListProperty() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        public ListProperty(string name, List<object> values) : base(name, values) {
            _list = values;
            if (values.Count == 0)
                this.Value = "INVALID";
            else
                this.Value = values[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
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
            set { 
                _list = value;
                OnPropertyChanged("List");
            }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index {
            get {
                if (Value == null) return -1;
                int index = 0;
                for (; index < _list.Count; index++) {
                    if (_list[index].ToString() == Value.ToString()) break;
                    else if (_list[index] == Value) break;
                }
                return index;
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public override Type Type {
            get {
                return typeof(List<object>);
            }
        }
    }
}
