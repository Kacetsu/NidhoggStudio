using ns.Communication.Models;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProcessorServiceClient : GenericDuplexServiceClient<IProcessorService, IProcessorServiceCallbacks>, IProcessorService {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="callbacks">The callbacks.</param>
        public ProcessorServiceClient(EndpointAddress endpoint, Binding binding, ProcessorServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(Guid.NewGuid().ToString());
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
    }
}