using ns.Base.Event;
using ns.Base.Manager;
using ns.Base.Manager.DataStorage;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ns.Core.Manager {

    public class DataStorageManager : NodeManager<DataContainer>, IDataStorageCollectionChangedEventHandler {
        private ConcurrentBag<DataContainer> _operationContainers = new ConcurrentBag<DataContainer>();
        private ConcurrentBag<ToolDataContainer> _toolContainers = new ConcurrentBag<ToolDataContainer>();

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
                _toolContainers.Add(node as ToolDataContainer);
            } else {
                _operationContainers.Add(node);
            }

            DataStorageCollectionChanged?.Invoke(this, new DataStorageCollectionChangedEventArgs(node.UID, node.ParentUID));
        }

        /// <summary>
        /// Finds the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DataContainer Find(string uid) {
            DataContainer container = null;

            Task<DataContainer> operationTask = FindOperationContainer(uid);
            Task<DataContainer> toolTask = FindToolContainer(uid);

            Task.WaitAll(operationTask, toolTask);

            container = operationTask.Result != null ? operationTask.Result : toolTask.Result;

            return container;
        }

        /// <summary>
        /// Finds the last.
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public DataContainer FindLast(string parentUID) {
            DataContainer container = null;
            Task<DataContainer> operationTask = Task.Factory.StartNew(() => _operationContainers.LastOrDefault(c => c.ParentUID == parentUID));
            Task<ToolDataContainer> toolTask = Task.Factory.StartNew(() => _toolContainers.LastOrDefault(c => c.ParentUID == parentUID));

            Task.WaitAll(operationTask, toolTask);

            container = operationTask.Result != null ? operationTask.Result : toolTask.Result;

            return container;
        }

        private Task<DataContainer> FindOperationContainer(string uid) {
            return Task.Factory.StartNew(() => {
                DataContainer container = null;
                foreach (OperationDataContainer operationContainer in _operationContainers) {
                    if (operationContainer.UID.Equals(uid)) {
                        container = operationContainer;
                        break;
                    }
                }
                return container;
            });
        }

        private Task<DataContainer> FindToolContainer(string uid) {
            return Task.Factory.StartNew(() => {
                DataContainer container = null;
                Parallel.ForEach(_toolContainers, (toolContainer, state) => {
                    if (toolContainer.UID.Equals(uid)) {
                        container = toolContainer;
                        state.Break();
                    }
                });
                return container;
            });
        }
    }
}