using ns.Base.Event;
using System.Collections.Generic;

namespace ns.Communication.Services.Callbacks {

    public class DataStorageServiceCallbacks : IDataStorageServiceCallbacks {

        /// <summary>
        /// Occurs when [data storage collection changed].
        /// </summary>
        public event DataStorageCollectionChangedEventHandler DataStorageCollectionChanged;

        /// <summary>
        /// Called when [data storage collection changed].
        /// </summary>
        /// <param name="newContainers">The new containers.</param>
        public void OnDataStorageCollectionChanged(Dictionary<string, string> newContainers) => DataStorageCollectionChanged?.Invoke(this, new DataStorageCollectionChangedEventArgs(newContainers));
    }
}