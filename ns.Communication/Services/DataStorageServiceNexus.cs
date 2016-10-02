using ns.Base.Log;
using ns.Base.Manager.DataStorage;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    internal class DataStorageServiceNexus : Nexus<IDataStorageServiceCallbacks> {
        private static Lazy<DataStorageServiceNexus> _instance = new Lazy<DataStorageServiceNexus>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageServiceNexus"/> class.
        /// </summary>
        public DataStorageServiceNexus() : base() {
            _dataStorageManager.DataStorageCollectionChanged += _dataStorageManager_DataStorageCollectionChanged;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static DataStorageContainerModel GetContainer(Guid clientId, Guid id) {
            CheckClientAvailable(clientId);
            DataContainer container = _instance.Value._dataStorageManager?.Find(id);
            if (container == null) throw new FaultException(string.Format("No container with uid {0} found!", id));
            return new DataStorageContainerModel(container);
        }

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public static DataStorageContainerModel GetLastContainer(Guid clientId, Guid parentId) {
            CheckClientAvailable(clientId);
            DataContainer container = _instance.Value._dataStorageManager?.FindLast(parentId);
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
        public static bool IsContainerAvailable(Guid parentId) {
            DataContainer container = _instance.Value._dataStorageManager?.FindLast(parentId);
            return container != null;
        }

        private static void CheckClientAvailable(Guid id) {
            if (_clients?.ContainsKey(id) == true) {
            } else {
                throw new FaultException(string.Format("Client is not registered {0}!", id));
            }
        }

        private void _dataStorageManager_DataStorageCollectionChanged(object sender, Base.Event.DataStorageCollectionChangedEventArgs e) {
            if (e.NewContainers.Count == 0) return;

            // Notify clients.
            List<Guid> damagedIds = new List<Guid>();
            Task.Factory.StartNew(() => {
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnDataStorageCollectionChanged(e.NewContainers);
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedIds.Add(client.Key);
                    }
                });

                RemoveDisconnectedClients(damagedIds);
            });
        }
    }
}