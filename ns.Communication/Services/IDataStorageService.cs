using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDataStorageServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IDataStorageService {

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        [OperationContract]
        DataStorageContainerModel GetContainer(string uid);

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        [OperationContract]
        DataStorageContainerModel GetLastContainer(string parentUID);

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        [OperationContract]
        bool IsContainerAvailable(string parentUID);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        [OperationContract]
        void RegisterClient(string uid);

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool SendHeartbeat();

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        [OperationContract]
        void UnregisterClient(string uid);
    }
}