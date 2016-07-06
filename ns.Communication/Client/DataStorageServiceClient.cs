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
            RegisterClient(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(string uid) => Channel?.GetContainer(uid);

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
    }
}