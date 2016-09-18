using System;

namespace ns.Communication.Events {

    public class PropertyChangedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        public PropertyChangedEventArgs() : base() {
            Id = Guid.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedEventArgs"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        public PropertyChangedEventArgs(Guid id) : this() {
            Id = id;
        }

        /// <summary>
        /// Gets the uid.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        public Guid Id { get; }
    }
}