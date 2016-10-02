using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    internal class Nexus<T> {
        protected static ConcurrentDictionary<Guid, T> _clients = new ConcurrentDictionary<Guid, T>();
        protected DataStorageManager _dataStorageManager;
        protected PluginManager _pluginManager;
        protected ProjectManager _projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Nexus"/> class.
        /// </summary>
        protected Nexus() {
            _projectManager = CoreSystem.FindManager<ProjectManager>();
            _pluginManager = CoreSystem.FindManager<PluginManager>();
            _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();
        }

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public static void RegisterClient(Guid id) {
            if (_clients.ContainsKey(id)) {
                throw new FaultException(string.Format("Client {0} already exists!", id));
            }

            _clients.TryAdd(id, OperationContext.Current.GetCallbackChannel<T>());
        }

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void UnregisterClient(Guid id) {
            T callback;
            _clients?.TryRemove(id, out callback);
        }

        /// <summary>
        /// Removes the disconnected clients.
        /// </summary>
        /// <param name="ids">The ids.</param>
        protected static void RemoveDisconnectedClients(ICollection<Guid> ids) {
            foreach (Guid damagedId in ids) {
                T callback;
                _clients.TryRemove(damagedId, out callback);
            }
        }
    }
}