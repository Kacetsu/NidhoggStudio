using ns.Communication.Models;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ProcessorService : IProcessorService {

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public ProcessorInfoModel GetState() => ProcessorServiceNexus.GetState();

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid id) => ProcessorServiceNexus.RegisterClient(id);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public void Start() => ProcessorServiceNexus.Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public void Stop() => ProcessorServiceNexus.Stop();

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(Guid id) => ProcessorServiceNexus.UnregisterClient(id);
    }
}