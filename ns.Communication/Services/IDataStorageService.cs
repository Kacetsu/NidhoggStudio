using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDataStorageServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IDataStorageService {

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [OperationContract]
        DataStorageContainerModel GetContainer(Guid clientId, Guid id);

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// /// <param name="clientId">The client id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        [OperationContract]
        DataStorageContainerModel GetLastContainer(Guid clientId, Guid parentId);

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        [OperationContract]
        bool IsContainerAvailable(Guid parentId);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract]
        void RegisterClient(Guid id);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract(IsOneWay = true)]
        void UnregisterClient(Guid id);
    }
}