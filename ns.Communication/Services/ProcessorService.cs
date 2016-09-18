using ns.Base.Log;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ProcessorService : IProcessorService {
        private ConcurrentDictionary<Guid, IProcessorServiceCallbacks> _clients = new ConcurrentDictionary<Guid, IProcessorServiceCallbacks>();

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public IProcessorServiceCallbacks Proxy { get { return OperationContext.Current.GetCallbackChannel<IProcessorServiceCallbacks>(); } }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public ProcessorInfoModel GetState() => new ProcessorInfoModel(CoreSystem.Processor);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid id) {
            if (_clients.ContainsKey(id)) {
                throw new FaultException(string.Format("Client {0} already exists!", id));
            }
            _clients.TryAdd(id, OperationContext.Current.GetCallbackChannel<IProcessorServiceCallbacks>());
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public void Start() {
            if (!CoreSystem.Processor.Start()) {
                throw new FaultException(string.Format("Could not start processor!"));
            } else {
                ConcurrentBag<Guid> damagedIds = new ConcurrentBag<Guid>();
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnProcessorStarted(new ProcessorInfoModel(CoreSystem.Processor));
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedIds.Add(client.Key);
                    }
                });

                foreach (Guid damagedId in damagedIds) {
                    IProcessorServiceCallbacks callback;
                    _clients.TryRemove(damagedId, out callback);
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public void Stop() {
            if (!CoreSystem.Processor.Stop()) {
                throw new FaultException(string.Format("Could not stop processor!"));
            } else {
                foreach (var client in _clients) {
                    client.Value?.OnProcessorStopped(new ProcessorInfoModel(CoreSystem.Processor));
                }
            }
        }

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(Guid id) {
            IProcessorServiceCallbacks callback;
            _clients?.TryRemove(id, out callback);
        }
    }
}