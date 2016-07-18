using ns.Base.Log;
using ns.Base.Manager;
using ns.Communication.Configuration;
using ns.Communication.Services;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace ns.Communication {

    public class CommunicationManager : GenericConfigurationManager<CommunicationConfiguration>, IGenericConfigurationManager<CommunicationConfiguration>, ICommunicationManager {
        private static Lazy<CommunicationManager> _lazyInstance = new Lazy<CommunicationManager>(() => new CommunicationManager());
        private List<Task> _serviceTasks = new List<Task>();
        private List<SemaphoreSlim> _stopSignals = new List<SemaphoreSlim>();
        private Uri _uri;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationManager"/> class.
        /// </summary>
        private CommunicationManager() : base() {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CommunicationManager Instance { get; } = _lazyInstance.Value;

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public override void Close() {
            try {
                Parallel.ForEach(_stopSignals, (stopSignal) => {
                    stopSignal.Release();
                    stopSignal.Wait();
                });
            } catch (Exception ex) when (ex is ObjectDisposedException || ex is SemaphoreFullException) {
                Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Error);
                throw;
            }

            base.Close();
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect() {
            if (IsConnected) return;
            Configuration = new CommunicationConfiguration();
            _uri = new Uri(Configuration.Address.Value);

            try {
                _uri = new Uri(Configuration.Address.Value);
                _serviceTasks.Add(new Task(StartPluginService));
                _serviceTasks.Add(new Task(StartProjectService));
                _serviceTasks.Add(new Task(StartProcessorService));
                _serviceTasks.Add(new Task(StartDataStorageService));

                foreach (Task service in _serviceTasks) {
                    service.Start();
                }

                IsConnected = true;
            } catch (Exception ex) when (ex is ArgumentNullException || ex is ObjectDisposedException || ex is InvalidOperationException) {
                Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Error);
                throw;
            }
        }

        private NetTcpBinding CreateNetTcpBinding() {
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = Configuration.MaxReceivedMessageSize;
            return binding;
        }

        private ServiceMetadataBehavior CreateServiceMetadataBehavior() {
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            return smb;
        }

        private void StartDataStorageService() {
            SemaphoreSlim stopSignal = new SemaphoreSlim(0, 1);
            _stopSignals.Add(stopSignal);
            StartService<DataStorageService, IDataStorageService>(Configuration.DataStorageServiceAddress, stopSignal);
        }

        private void StartPluginService() {
            SemaphoreSlim stopSignal = new SemaphoreSlim(0, 1);
            _stopSignals.Add(stopSignal);
            StartService<PluginService, IPluginService>(Configuration.PluginServiceAddress, stopSignal);
        }

        private void StartProcessorService() {
            SemaphoreSlim stopSignal = new SemaphoreSlim(0, 1);
            _stopSignals.Add(stopSignal);
            StartService<ProcessorService, IProcessorService>(Configuration.ProcessorServiceAddress, stopSignal);
        }

        private void StartProjectService() {
            SemaphoreSlim stopSignal = new SemaphoreSlim(0, 1);
            _stopSignals.Add(stopSignal);
            StartService<ProjectService, IProjectService>(Configuration.ProjectServiceAddress, stopSignal);
        }

        private void StartService<T, U>(string address, SemaphoreSlim semaphore) where T : class {
            Uri baseAddress = new Uri(address);

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(T), baseAddress)) {
                NetTcpBinding binding = CreateNetTcpBinding();
                host.AddServiceEndpoint(typeof(U), binding, baseAddress);
                host.Description.Behaviors.Add(CreateServiceMetadataBehavior());
                host.Open();
                semaphore.Wait();
                host.Close();
            }

            semaphore = new SemaphoreSlim(0, 1);
        }
    }
}