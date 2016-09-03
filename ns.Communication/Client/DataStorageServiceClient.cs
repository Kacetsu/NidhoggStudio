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
            RegisterClient(ClientUid);
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientUid">The client uid.</param>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(string clientUid, string uid) => Channel?.GetContainer(clientUid, uid);

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientUid">The client uid.</param>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetLastContainer(string clientUid, string parentUID) => Channel?.GetLastContainer(clientUid, parentUID);

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public bool IsContainerAvailable(string parentUID) => Channel?.IsContainerAvailable(parentUID) == true;

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void RegisterClient(string uid) => Channel?.RegisterClient(uid);

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <returns></returns>
        public bool SendHeartbeat() => Channel?.SendHeartbeat() == true;

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void UnregisterClient(string uid) => Channel?.UnregisterClient(uid);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            UnregisterClient(ClientUid);
            base.Dispose(disposing);
        }
    }
}