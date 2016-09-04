﻿using ns.Base.Log;
using ns.Base.Manager.DataStorage;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class DataStorageService : IDataStorageService {
        private ConcurrentDictionary<string, IDataStorageServiceCallbacks> _clients = new ConcurrentDictionary<string, IDataStorageServiceCallbacks>();
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
        /// <param name="clientUid">The client uid.</param>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetContainer(string clientUid, string uid) {
            CheckClientAvailable(clientUid);
            DataContainer container = _dataStorageManager?.Find(uid);
            if (container == null) throw new FaultException(string.Format("No container with uid {0} found!", uid));
            return new DataStorageContainerModel(container);
        }

        /// <summary>
        /// Gets the last container.
        /// </summary>
        /// <param name="clientUid">The client uid.</param>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public DataStorageContainerModel GetLastContainer(string clientUid, string parentUID) {
            CheckClientAvailable(clientUid);
            DataContainer container = _dataStorageManager?.FindLast(parentUID);
            if (container == null) {
                throw new FaultException(string.Format("No container available! {0}", parentUID));
            }

            return new DataStorageContainerModel(container);
        }

        /// <summary>
        /// Determines whether [is container available] [the specified parent uid].
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public bool IsContainerAvailable(string parentUID) {
            DataContainer container = _dataStorageManager?.FindLast(parentUID);
            return container != null;
        }

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(string uid) {
            if (_clients.ContainsKey(uid)) {
                throw new FaultException(string.Format("Client {0} already exists!", uid));
            }
            _clients.TryAdd(uid, OperationContext.Current.GetCallbackChannel<IDataStorageServiceCallbacks>());
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
        /// <param name="uid">The uid.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(string uid) {
            IDataStorageServiceCallbacks callback;
            _clients?.TryRemove(uid, out callback);
        }

        private void _dataStorageManager_DataStorageCollectionChanged(object sender, Base.Event.DataStorageCollectionChangedEventArgs e) {
            if (e.NewContainers.Count == 0) return;

            // Notify clients.
            ConcurrentBag<string> damagedUIDs = new ConcurrentBag<string>();
            Task.Factory.StartNew(() => {
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnDataStorageCollectionChanged(e.NewContainers);
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedUIDs.Add(client.Key);
                    }
                });

                foreach (string damagedUID in damagedUIDs) {
                    IDataStorageServiceCallbacks callback;
                    _clients.TryRemove(damagedUID, out callback);
                }
            });
        }

        private void CheckClientAvailable(string uid) {
            if (_clients?.ContainsKey(uid) == true) {
            } else {
                throw new FaultException(string.Format("Client is not registered {0}!", uid));
            }
        }
    }
}