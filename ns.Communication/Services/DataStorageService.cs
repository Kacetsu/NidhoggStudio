using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class DataStorageService : IDataStorageService {
        private Dictionary<string, IDataStorageServiceCallbacks> _clients = new Dictionary<string, IDataStorageServiceCallbacks>();
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
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(string uid) => new DataStorageContainerModel(_dataStorageManager?.Find(uid));

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(string uid) {
            if (_clients.ContainsKey(uid)) {
                throw new FaultException(string.Format("Client [0] already exists!", uid));
            }
            _clients.Add(uid, OperationContext.Current.GetCallbackChannel<IDataStorageServiceCallbacks>());
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

        private void _dataStorageManager_DataStorageCollectionChanged(object sender, Base.Event.DataStorageCollectionChangedEventArgs e) {
            if (e.NewUIDs.Count == 0) return;

            string newUID = e.NewUIDs[0];

            foreach (var client in _clients) {
                client.Value?.OnDataStorageCollectionChanged(newUID);
            }
        }
    }
}