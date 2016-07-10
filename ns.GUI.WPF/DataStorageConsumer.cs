using ns.Base.Event;
using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ns.GUI.WPF {

    internal sealed class DataStorageConsumer {
        private BlockingCollection<KeyValuePair<string, string>> _blockingDataStorageModelUIDs = new BlockingCollection<KeyValuePair<string, string>>();
        private Task _uidTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageConsumer"/> class.
        /// </summary>
        public DataStorageConsumer() {
            ClientCommunicationManager.DataStorageService.Callback.DataStorageCollectionChanged += Callback_DataStorageCollectionChanged;
            _uidTask = new Task(UpdateDataStorage);
            _uidTask.Start();
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

        private void Callback_DataStorageCollectionChanged(object sender, DataStorageCollectionChangedEventArgs e) {
            foreach (KeyValuePair<string, string> container in e.NewContainers) {
                Add(container);
            }
        }

        private void UpdateDataStorage() {
            KeyValuePair<string, string> container;
            while (true) {
                container = _blockingDataStorageModelUIDs.Take();
                string containerUID = container.Key;
                string pluginUID = container.Value;
                if (!string.IsNullOrEmpty(SelectedUID) && !string.IsNullOrEmpty(pluginUID) && pluginUID.Equals(SelectedUID) && !string.IsNullOrEmpty(containerUID)) {
                    DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetContainer(containerUID);
                    DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                }
            }
        }
    }
}