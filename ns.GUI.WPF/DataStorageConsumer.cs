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
        private BlockingCollection<KeyValuePair<Guid, Guid>> _blockingDataStorageModelUIDs = new BlockingCollection<KeyValuePair<Guid, Guid>>();
        private BlockingCollection<Guid> _blockingLastDataStorageParentUIDs = new BlockingCollection<Guid>();
        private CancellationToken _cancellationTokenModelUIDs;
        private CancellationToken _cancellationTokenParentUIDs;
        private CancellationTokenSource _cancellationTokenSourceModelUIDs = new CancellationTokenSource();
        private CancellationTokenSource _cancellationTokenSourceParentIds = new CancellationTokenSource();
        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageConsumer"/> class.
        /// </summary>
        public DataStorageConsumer() {
            ClientCommunicationManager.DataStorageService.Callback.DataStorageCollectionChanged += Callback_DataStorageCollectionChanged;
            _cancellationTokenParentUIDs = _cancellationTokenSourceParentIds.Token;
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
        public Guid SelectedId { get; set; }

        /// <summary>
        /// Adds the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Add(KeyValuePair<Guid, Guid> container) {
            while (_blockingDataStorageModelUIDs.Count > 10) {
                _blockingDataStorageModelUIDs.Take();
            }

            if (!_blockingDataStorageModelUIDs.IsAddingCompleted) {
                _blockingDataStorageModelUIDs.Add(container);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            _blockingDataStorageModelUIDs?.CompleteAdding();
            _blockingLastDataStorageParentUIDs?.CompleteAdding();
            _cancellationTokenSourceModelUIDs?.CancelAfter(TimeoutMs + 100);
            _cancellationTokenSourceParentIds?.CancelAfter(TimeoutMs + 100);

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
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public void UpdateLastDataStorage(Guid parentId) {
            _blockingLastDataStorageParentUIDs.Add(parentId);
        }

        private void Callback_DataStorageCollectionChanged(object sender, DataStorageCollectionChangedEventArgs e) {
            foreach (KeyValuePair<Guid, Guid> container in e.NewContainers) {
                Add(container);
            }
        }

        private void Dispose(bool disposing) {
            if (!disposing) return;

            Close();
            _blockingDataStorageModelUIDs?.Dispose();
            _blockingLastDataStorageParentUIDs?.Dispose();
            _cancellationTokenSourceModelUIDs?.Dispose();
            _cancellationTokenSourceParentIds?.Dispose();
        }

        private void UpdateDataStorage() {
            KeyValuePair<Guid, Guid> container;
            while (!_blockingDataStorageModelUIDs.IsCompleted && !_cancellationTokenSourceModelUIDs.IsCancellationRequested) {
                if (_blockingDataStorageModelUIDs.TryTake(out container, TimeoutMs)) {
                    if (Guid.Empty.Equals(container.Value)) continue;
                    Guid containerId = container.Key;
                    Guid pluginId = container.Value;
                    if (!Guid.Empty.Equals(SelectedId) && !Guid.Empty.Equals(pluginId) && pluginId.Equals(SelectedId) && !Guid.Empty.Equals(containerId)) {
                        DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetContainer(DataStorageServiceClient.ClientId, containerId);
                        DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                    }
                }
            }
        }

        private void UpdateLastDataStorage() {
            while (!_blockingLastDataStorageParentUIDs.IsCompleted && !_cancellationTokenSourceParentIds.IsCancellationRequested) {
                Guid parentId;
                if (_blockingLastDataStorageParentUIDs.TryTake(out parentId, TimeoutMs)) {
                    if (Guid.Empty.Equals(parentId)) continue;
                    try {
                        if (ClientCommunicationManager.DataStorageService.IsContainerAvailable(parentId)) {
                            DataStorageContainerModel dataModel = ClientCommunicationManager.DataStorageService.GetLastContainer(DataStorageServiceClient.ClientId, parentId);
                            DataStorageAdded?.Invoke(this, new DataStorageContainerModelAddedEventArgs(dataModel));
                        }
                    } catch (FaultException ex) {
                        Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Warning);
                    }
                }
            }
        }
    }
}