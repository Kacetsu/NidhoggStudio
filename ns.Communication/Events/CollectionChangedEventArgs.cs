using System;
using System.Collections.Generic;

namespace ns.Communication.Events {

    public class CollectionChangedEventArgs : EventArgs {

        /// <summary>
        /// Gets the new objects.
        /// </summary>
        /// <value>
        /// The new objects.
        /// </value>
        public IEnumerable<object> NewObjects { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
        /// </summary>
        public CollectionChangedEventArgs() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newObjects">The new objects.</param>
        public CollectionChangedEventArgs(IEnumerable<object> newObjects) : this() {
            NewObjects = newObjects;
        }
    }
}