using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class DataStorageCollectionChangedEventArgs : EventArgs {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="containerId">The container identifier.</param>
        /// <param name="pluginId">The plugin identifier.</param>
        public DataStorageCollectionChangedEventArgs(Guid containerId, Guid pluginId) : base() {
            NewContainers = new Dictionary<Guid, Guid>();
            NewContainers.Add(containerId, pluginId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="containerDictionary">The container dictionary.</param>
        public DataStorageCollectionChangedEventArgs(Dictionary<Guid, Guid> containerDictionary) : base() {
            NewContainers = containerDictionary;
        }

        /// <summary>
        /// Gets the new containers.
        /// </summary>
        /// <value>
        /// The new containers.
        /// </value>
        public Dictionary<Guid, Guid> NewContainers { get; private set; }
    }
}