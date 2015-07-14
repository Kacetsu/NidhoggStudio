using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class NodeChangedEventArgs : EventArgs {

        private string _name = string.Empty;
        private object _value = null;
        private object _oldValue = null;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// Gets the property that rised the event.
        /// </summary>
        public object Value {
            get { return _value; }
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public object OldValue {
            get { return _oldValue; }
        }

        /// <summary>
        /// Handles the property informations that rised the event.
        /// </summary>
        /// <param name="node">The rising property.</param>
        public NodeChangedEventArgs(Node node)
            : base() {
                _value = node;
                _name = _value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public NodeChangedEventArgs(string name, object value)
            : base() {
                _name = name;
                _value = value;
        }
    }
}
