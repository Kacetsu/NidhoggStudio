using ns.Communication.Event;

namespace ns.Communication.Services.Callbacks {

    public class DataStorageServiceCallbacks : IDataStorageServiceCallbacks {

        /// <summary>
        /// Occurs when [data storage collection changed].
        /// </summary>
        public event DataStorageCollectionChangedEventHandler DataStorageCollectionChanged;

        /// <summary>
        /// Called when [data storage collection changed].
        /// </summary>
        /// <param name="newUID"></param>
        public void OnDataStorageCollectionChanged(string newUID) => DataStorageCollectionChanged?.Invoke(this, new DataStorageCollectionChangedEventArgs(newUID));
    }
}