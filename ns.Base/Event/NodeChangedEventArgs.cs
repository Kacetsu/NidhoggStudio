using System;

namespace ns.Base.Event {

    public class NodeChangedEventArgs : EventArgs {
        private string _name = string.Empty;
        private object _oldValue = null;
        private object _value = null;

        /// <summary>
        /// Handles the property informations that rised the event.
        /// </summary>
        /// <param name="node">The rising property.</param>
        public NodeChangedEventArgs(Node node)
            : base() {
            if (node == null) throw new ArgumentNullException(nameof(node));

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
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            _name = name;
            _value = value;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => _name;

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public object OldValue => _oldValue;

        /// <summary>
        /// Gets the property that rised the event.
        /// </summary>
        public object Value => _value;
    }
}