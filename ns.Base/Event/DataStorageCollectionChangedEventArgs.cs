using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class DataStorageCollectionChangedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newUID">The new uid.</param>
        public DataStorageCollectionChangedEventArgs(string newUID) : base() {
            NewUIDs = new List<string> { newUID };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newUIDs">The new UI ds.</param>
        public DataStorageCollectionChangedEventArgs(List<string> newUIDs) : base() {
            NewUIDs = newUIDs;
        }

        /// <summary>
        /// Gets the new UI ds.
        /// </summary>
        /// <value>
        /// The new UI ds.
        /// </value>
        public List<string> NewUIDs { get; private set; }
    }
}