using ns.Base;
using ns.Base.Event;
using ns.Base.Log;
using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ns.GUI.WPF {

    internal sealed class DataStorageConsumer : ICloseable, IDisposable {
        private const int TimeoutMs = 500;
        private BlockingCollection<KeyValuePair<string, string>> _blockingDataStorageModelUIDs = new BlockingCollection<KeyValuePair<string, string>>();
        private BlockingCollection<string> _blockingLastDataStorageParentUIDs = new BlockingCollection<string>();
        private CancellationToken _cancellationTokenModelUIDs;
        private CancellationToken _cancellationTokenParentUIDs;
        private CancellationTokenSource _cancellationTokenSourceModelUIDs = new CancellationTokenSource();
        private CancellationTokenSource _cancellationTokenSourceParentUIDs = new CancellationTokenSource();
        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageConsumer"/> class.
        /// </summary>
        public DataStorageConsumer() {
            ClientCommunicationManager.DataStorageService.Callback.DataStorageCollectionChanged += Callback_DataStorageCollectionChanged;
            _cancellationTokenParentUIDs = _cancellationTokenSourceParentUIDs.Token;
            _cancellationTokenModelUIDs = _cancellationTokenSourceModelUIDs.Token;
            TaskFactory factoryParentUIDs = new TaskFactory(_cancellationTokenParentUIDs);
            TaskFactory factoryModelUIDs = new TaskFactory(_cancellationTokenModelUIDs);
            _tasks.Add(factoryModelUIDs.StartNew(() => UpdateDataStorage()));
            _tasks.Add(factoryParentUIDs.StartNew(() => UpdateLastDataStorage()));
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
            _blockingDataStorageModelUIDs?.CompleteAdding();
            _cancellationTokenSourceModelUIDs?.CancelAfter(TimeoutMs + 100);
            _blockingLastDataStorageParentUIDs?.CompleteAdding();
            _cancellationTokenSourceParentUIDs?.CancelAfter(TimeoutMs + 100);

            Task.WaitAll(_tasks.ToArray(), TimeoutMs + 200);
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        private void Dispose(bool disposing) {
            if (disposing) {
                Close();
                _blockingDataStorageModelUIDs?.Dispose();
                _blockingLastDataStorageParentUIDs?.Dispose();
                _cancellationTokenSourceModelUIDs?.Dispose();
                _cancellationTokenSourceParentUIDs?.Dispose();
            }
        }

        private void UpdateDataStorage() {
            KeyValuePair<string, string> container;
            while (!_blockingDataStorageModelUIDs.IsCompleted && !_cancellationTokenSourceModelUIDs.IsCancellationRequested) {
                _blockingDataStorageModelUIDs.TryTake(out container, TimeoutMs, _cancellationTokenModelUIDs);
                if (string.IsNullOrEmpty(container.Value)) continue;
                string containerUID = container.Key;
                string pluginUID = container.Value;
                if (!string.IsNullOrEmpty(SelectedUID) && !string.IsNullOrEmpty(pluginUID) && pluginUID.Equals(SelectedUID) && !string.IsNullOrEmpty(containerUID)) {
                    DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetContainer(containerUID);
                    DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                }
            }
        }

        private void UpdateLastDataStorage() {
            while (!_blockingLastDataStorageParentUIDs.IsCompleted && !_cancellationTokenSourceParentUIDs.IsCancellationRequested) {
                string parentUID;
                _blockingLastDataStorageParentUIDs.TryTake(out parentUID, TimeoutMs, _cancellationTokenParentUIDs);
                if (string.IsNullOrEmpty(parentUID)) continue;
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