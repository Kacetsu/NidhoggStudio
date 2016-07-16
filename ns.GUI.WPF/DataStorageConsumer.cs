using ns.Base;
using ns.Base.Event;
using ns.Base.Log;
using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.GUI.WPF {

    internal sealed class DataStorageConsumer : ICloseable {
        private BlockingCollection<KeyValuePair<string, string>> _blockingDataStorageModelUIDs = new BlockingCollection<KeyValuePair<string, string>>();
        private BlockingCollection<string> _blockingLastDataStorageParentUIDs = new BlockingCollection<string>();
        private bool _isClosing = false;
        private Task _parentUidTask;
        private Task _uidTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageConsumer"/> class.
        /// </summary>
        public DataStorageConsumer() {
            ClientCommunicationManager.DataStorageService.Callback.DataStorageCollectionChanged += Callback_DataStorageCollectionChanged;
            _uidTask = new Task(UpdateDataStorage);
            _parentUidTask = new Task(UpdateLastDataStorage);
            _uidTask.Start();
            _parentUidTask.Start();
        }

        public delegate void DataStorageAddedHandler(object sender, DataStorageContainerModelAddedEventArgs e);

        /// <summary>
        /// Occurs when [data storage added].
        /// </summary>
        public event DataStorageAddedHandler DataStorageAdded;

        /// <summary>
        /// Gets or sets the selected uid.
        /// </summary>
        /// <value>
        /// The selected uid.
        /// </value>
        public string SelectedUID { get; set; }

        /// <summary>
        /// Adds the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Add(KeyValuePair<string, string> container) {
            while (_blockingDataStorageModelUIDs.Count > 10) {
                _blockingDataStorageModelUIDs.Take();
            }
            _blockingDataStorageModelUIDs.Add(container);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            _isClosing = true;
            _blockingDataStorageModelUIDs?.CompleteAdding();
            _blockingLastDataStorageParentUIDs?.CompleteAdding();
        }

        /// <summary>
        /// Updates the last data storage.
        /// </summary>
        /// <param name="parentUID">The parent uid.</param>
        /// <returns></returns>
        public void UpdateLastDataStorage(string parentUID) {
            _blockingLastDataStorageParentUIDs.Add(parentUID);
        }

        private void Callback_DataStorageCollectionChanged(object sender, DataStorageCollectionChangedEventArgs e) {
            foreach (KeyValuePair<string, string> container in e.NewContainers) {
                Add(container);
            }
        }

        private void UpdateDataStorage() {
            KeyValuePair<string, string> container;
            while (!_isClosing) {
                container = _blockingDataStorageModelUIDs.Take();
                string containerUID = container.Key;
                string pluginUID = container.Value;
                if (!string.IsNullOrEmpty(SelectedUID) && !string.IsNullOrEmpty(pluginUID) && pluginUID.Equals(SelectedUID) && !string.IsNullOrEmpty(containerUID)) {
                    DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetContainer(containerUID);
                    DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                }
            }
        }

        private void UpdateLastDataStorage() {
            while (!_isClosing) {
                string parentUID = _blockingLastDataStorageParentUIDs.Take();
                try {
                    if (ClientCommunicationManager.DataStorageService.IsContainerAvailable(parentUID)) {
                        DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetLastContainer(parentUID);
                        DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                    }
                } catch (FaultException ex) {
                    Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Warning);
                }
            }
        }
    }
}