using ns.Base.Event;
using System;
using System.Collections.Generic;

namespace ns.Communication.Services.Callbacks {

    public class DataStorageServiceCallbacks : IDataStorageServiceCallbacks {
        /// <summary>
        /// Occurs when [data storage collection changed].
        /// </summary>
        //public event DataStorageCollectionChangedEventHandler DataStorageCollectionChanged;

        public event Base.Event.EventHandler<DataStorageCollectionChangedEventArgs> DataStorageCollectionChanged;

        /// <summary>
        /// Called when [data storage collection changed].
        /// </summary>
        /// <param name="newContainers">The new containers.</param>
        public void OnDataStorageCollectionChanged(Dictionary<Guid, Guid> newContainers) => DataStorageCollectionChanged?.Invoke(this, new DataStorageCollectionChangedEventArgs(newContainers));
    }
}