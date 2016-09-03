namespace ns.Base.Event {

    public interface IDataStorageCollectionChangedEventHandler {

        /// <summary>
        /// Occurs when [data storage collection changed].
        /// </summary>
        event EventHandler<DataStorageCollectionChangedEventArgs> DataStorageCollectionChanged;
    }
}