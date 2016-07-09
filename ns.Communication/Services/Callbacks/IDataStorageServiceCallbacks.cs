using ns.Base.Event;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    public interface IDataStorageServiceCallbacks : IDataStorageCollectionChangedEventHandler {

        /// <summary>
        /// Called when [data storage collection changed].
        /// </summary>
        /// <param name="newContainers">The new containers.</param>
        [OperationContract(IsOneWay = true)]
        void OnDataStorageCollectionChanged(Dictionary<string, string> newContainers);
    }
}