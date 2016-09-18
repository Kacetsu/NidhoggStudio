using ns.Base.Log;
using ns.Communication.Models;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProcessorServiceClient : GenericDuplexServiceClient<IProcessorService, IProcessorServiceCallbacks>, IProcessorService {
        private Guid _clientId = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="callbacks">The callbacks.</param>
        public ProcessorServiceClient(EndpointAddress endpoint, Binding binding, ProcessorServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(_clientId);
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public ProcessorInfoModel GetState() => Channel?.GetState();

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RegisterClient(Guid id) => Channel?.RegisterClient(id);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start() => Channel?.Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            try {
                Channel?.Stop();
            } catch (Exception ex) {
                Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Error);
            }
        }

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
            UnregisterClient(_clientId);
            base.Dispose(disposing);
        }
    }
}