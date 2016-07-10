using ns.Base.Event;
using ns.Base.Manager;
using ns.Base.Manager.DataStorage;
using System.Collections.Generic;

namespace ns.Core.Manager {

    public class DataStorageManager : NodeManager<DataContainer>, IDataStorageCollectionChangedEventHandler {
        private List<string> _containerUIDs = new List<string>();
        private List<ToolDataContainer> _toolContainers = new List<ToolDataContainer>();

        /// <summary>
        /// Occurs when [data storage collection changed].
        /// </summary>
        public event DataStorageCollectionChangedEventHandler DataStorageCollectionChanged;

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(DataContainer node) {
            if (node is ToolDataContainer) {
                lock (_toolContainers) {
                    _toolContainers.Add(node as ToolDataContainer);
                }
            } else {
                lock (Nodes) {
                    base.Add(node);
                }
            }

            lock (_containerUIDs) {
                _containerUIDs.Add(node.UID);
            }

            DataStorageCollectionChanged?.Invoke(this, new DataStorageCollectionChangedEventArgs(node.UID, node.ParentUID));
        }

        /// <summary>
        /// Finds the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataContainer Find(string uid) {
            lock (_toolContainers) {
                foreach (ToolDataContainer toolContainer in _toolContainers) {
                    if (toolContainer.UID.Equals(uid)) {
                        return toolContainer;
                    }
                }
            }

            lock (Nodes) {
                foreach (OperationDataContainer operationContainer in Nodes) {
                    if (operationContainer.UID.Equals(uid)) {
                        return operationContainer;
                    }
                }
            }

            return null;
        }
    }
}