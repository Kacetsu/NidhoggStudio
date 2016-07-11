using ns.Communication.Models;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProcessorServiceClient : GenericDuplexServiceClient<IProcessorService, IProcessorServiceCallbacks>, IProcessorService {
        private string _clientUID = Guid.NewGuid().ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="callbacks">The callbacks.</param>
        public ProcessorServiceClient(EndpointAddress endpoint, Binding binding, ProcessorServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(_clientUID);
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public ProcessorInfoModel GetState() => Channel?.GetState();

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void RegisterClient(string uid) => Channel?.RegisterClient(uid);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() => Channel?.Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() => Channel?.Stop();

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
            UnregisterClient(_clientUID);
            base.Dispose(disposing);
        }
    }
}