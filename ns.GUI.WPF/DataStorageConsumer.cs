using ns.Communication.Client;
using ns.Communication.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.GUI.WPF {

    public sealed class DataStorageConsumer {
        private BlockingCollection<string> _blockingDataStorageModelUIDs = new BlockingCollection<string>();
        private Task _uidTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageConsumer"/> class.
        /// </summary>
        public DataStorageConsumer() {
            ClientCommunicationManager.DataStorageService.Callback.DataStorageCollectionChanged += Callback_DataStorageCollectionChanged;
            _uidTask = new Task(UpdateDataStorage);
            _uidTask.Start();
        }

        /// <summary>
        /// Adds the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Add(string uid) {
            while (_blockingDataStorageModelUIDs.Count > 10) {
                _blockingDataStorageModelUIDs.Take();
            }
            _blockingDataStorageModelUIDs.Add(uid);
        }

        private void Callback_DataStorageCollectionChanged(object sender, Communication.Event.DataStorageCollectionChangedEventArgs e) {
            foreach (string uid in e.NewUIDs) {
                Add(uid);
            }
        }

        private void UpdateDataStorage() {
            string uid = string.Empty;
            while (true) {
                uid = _blockingDataStorageModelUIDs.Take();
                if (!string.IsNullOrEmpty(uid)) {
                    DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetContainer(uid);
                }
            }
        }
    }
}