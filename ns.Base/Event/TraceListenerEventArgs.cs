using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class TraceListenerEventArgs : EventArgs {
        private string _timestamp;
        private string _message;
        private string _category;

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public string Timestamp { get { return _timestamp; } }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get { return _message; } }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category { get { return _category; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerEventArgs"/> class.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        public TraceListenerEventArgs (string timestamp, string message, string category) {
            _timestamp = timestamp;
            _message = message;
            _category = category;
        }

    }
}
