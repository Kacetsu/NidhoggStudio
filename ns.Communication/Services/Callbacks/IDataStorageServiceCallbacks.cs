using ns.Communication.Event;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    public interface IDataStorageServiceCallbacks : IDataStorageCollectionChangedEventHandler {

        /// <summary>
        /// Called when [data storage collection changed].
        /// </summary>
        /// <param name="dataStorageContainerModel">The data storage container model.</param>
        [OperationContract(IsOneWay = true)]
        void OnDataStorageCollectionChanged(string newUID);
    }
}