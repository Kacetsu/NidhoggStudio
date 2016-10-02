using ns.Communication.Models;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class DataStorageService : IDataStorageService {

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(Guid clientId, Guid id) => DataStorageServiceNexus.GetContainer(clientId, id);

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetLastContainer(Guid clientId, Guid parentId) => DataStorageServiceNexus.GetLastContainer(clientId, parentId);

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public bool IsContainerAvailable(Guid parentId) => DataStorageServiceNexus.IsContainerAvailable(parentId);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid id) => DataStorageServiceNexus.RegisterClient(id);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void UnregisterClient(Guid id) => DataStorageServiceNexus.UnregisterClient(id);
    }
}