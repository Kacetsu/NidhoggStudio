using ns.Communication.Models;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class DataStorageServiceClient : GenericDuplexServiceClient<IDataStorageService, IDataStorageServiceCallbacks>, IDataStorageService {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="callbacks">The callbacks.</param>
        public DataStorageServiceClient(EndpointAddress endpoint, Binding binding, DataStorageServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(ClientId);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(Guid clientId, Guid id) => Channel?.GetContainer(clientId, id);

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetLastContainer(Guid clientId, Guid parentId) => Channel?.GetLastContainer(clientId, parentId);

        /// <summary>
        /// Determines whether [is container available] [the specified parent id].
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public bool IsContainerAvailable(Guid parentId) => Channel?.IsContainerAvailable(parentId) == true;

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RegisterClient(Guid id) => Channel?.RegisterClient(id);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        public void UnregisterClient(Guid id) => Channel?.UnregisterClient(id);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            UnregisterClient(ClientId);
            base.Dispose(disposing);
        }
    }
}