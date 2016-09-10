using System;

namespace ns.Communication.Events {

    public class PropertyChangedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        public PropertyChangedEventArgs() : base() {
            Uid = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public PropertyChangedEventArgs(string uid) : this() {
            Uid = uid;
        }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        public string Uid { get; }
    }
}