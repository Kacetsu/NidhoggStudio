using ns.Base.Log;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ProcessorService : IProcessorService {
        private ConcurrentDictionary<string, IProcessorServiceCallbacks> _clients = new ConcurrentDictionary<string, IProcessorServiceCallbacks>();

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
        /// <param name="uid">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(string uid) {
            if (_clients.ContainsKey(uid)) {
                throw new FaultException(string.Format("Client {0} already exists!", uid));
            }
            _clients.TryAdd(uid, OperationContext.Current.GetCallbackChannel<IProcessorServiceCallbacks>());
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public void Start() {
            if (!CoreSystem.Processor.Start()) {
                throw new FaultException(string.Format("Could not start processor!"));
            } else {
                ConcurrentBag<string> damagedUIDs = new ConcurrentBag<string>();
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnProcessorStarted(new ProcessorInfoModel(CoreSystem.Processor));
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedUIDs.Add(client.Key);
                    }
                });

                foreach (string damagedUID in damagedUIDs) {
                    IProcessorServiceCallbacks callback;
                    _clients.TryRemove(damagedUID, out callback);
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
        /// <param name="uid">The uid.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(string uid) {
            IProcessorServiceCallbacks callback;
            _clients?.TryRemove(uid, out callback);
        }
    }
}