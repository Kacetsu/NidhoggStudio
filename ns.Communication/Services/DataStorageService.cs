using ns.Base.Log;
using ns.Base.Manager.DataStorage;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class DataStorageService : IDataStorageService {
        private ConcurrentDictionary<Guid, IDataStorageServiceCallbacks> _clients = new ConcurrentDictionary<Guid, IDataStorageServiceCallbacks>();
        private DataStorageManager _dataStorageManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageService"/> class.
        /// </summary>
        public DataStorageService() {
            _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();
            _dataStorageManager.DataStorageCollectionChanged += _dataStorageManager_DataStorageCollectionChanged;
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public IDataStorageServiceCallbacks Proxy { get { return OperationContext.Current.GetCallbackChannel<IDataStorageServiceCallbacks>(); } }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(Guid clientId, Guid id) {
            CheckClientAvailable(clientId);
            DataContainer container = _dataStorageManager?.Find(id);
            if (container == null) throw new FaultException(string.Format("No container with uid {0} found!", id));
            return new DataStorageContainerModel(container);
        }

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetLastContainer(Guid clientId, Guid parentId) {
            CheckClientAvailable(clientId);
            DataContainer container = _dataStorageManager?.FindLast(parentId);
            if (container == null) {
                throw new FaultException(string.Format("No container available! {0}", parentId));
            }

            return new DataStorageContainerModel(container);
        }

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public bool IsContainerAvailable(Guid parentId) {
            DataContainer container = _dataStorageManager?.FindLast(parentId);
            return container != null;
        }

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid id) {
            if (_clients.ContainsKey(id)) {
                throw new FaultException(string.Format("Client {0} already exists!", id));
            }
            _clients.TryAdd(id, OperationContext.Current.GetCallbackChannel<IDataStorageServiceCallbacks>());
        }

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        /// <returns></returns>
        public bool SendHeartbeat() {
            if (_dataStorageManager == null) {
                _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();
                _dataStorageManager.DataStorageCollectionChanged -= _dataStorageManager_DataStorageCollectionChanged;
                _dataStorageManager.DataStorageCollectionChanged += _dataStorageManager_DataStorageCollectionChanged;
            }

            IDataStorageServiceCallbacks proxy = OperationContext.Current.GetCallbackChannel<IDataStorageServiceCallbacks>();

            return true;
        }

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void UnregisterClient(Guid id) {
            IDataStorageServiceCallbacks callback;
            _clients?.TryRemove(id, out callback);
        }

        private void _dataStorageManager_DataStorageCollectionChanged(object sender, Base.Event.DataStorageCollectionChangedEventArgs e) {
            if (e.NewContainers.Count == 0) return;

            // Notify clients.
            ConcurrentBag<Guid> damagedIds = new ConcurrentBag<Guid>();
            Task.Factory.StartNew(() => {
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnDataStorageCollectionChanged(e.NewContainers);
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedIds.Add(client.Key);
                    }
                });

                foreach (Guid damagedId in damagedIds) {
                    IDataStorageServiceCallbacks callback;
                    _clients.TryRemove(damagedId, out callback);
                }
            });
        }

        private void CheckClientAvailable(Guid id) {
            if (_clients?.ContainsKey(id) == true) {
            } else {
                throw new FaultException(string.Format("Client is not registered {0}!", id));
            }
        }
    }
}