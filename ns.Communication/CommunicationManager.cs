using ns.Base.Log;
using ns.Base.Manager;
using ns.Communication.Configuration;
using ns.Communication.Services;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace ns.Communication {

    public class CommunicationManager : GenericConfigurationManager<CommunicationConfiguration>, IGenericConfigurationManager<CommunicationConfiguration> {
        private static Lazy<CommunicationManager> _lazyInstance = new Lazy<CommunicationManager>(() => new CommunicationManager());

        private Task _serviceTask;
        private SemaphoreSlim _serviceStopSignal = new SemaphoreSlim(0, 1);
        private Uri _uri;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CommunicationManager Instance { get; } = _lazyInstance.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationManager"/> class.
        /// </summary>
        public CommunicationManager() : base() {
            Configuration = new CommunicationConfiguration();
            _uri = new Uri(Configuration.Address.Value);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();
            try {
                _uri = new Uri(Configuration.Address.Value);
                _serviceTask = new Task(StartService);
                _serviceTask.Start();
            } catch (Exception ex) when (ex is ArgumentNullException || ex is ObjectDisposedException || ex is InvalidOperationException) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Finalize() {
            try {
                _serviceStopSignal.Release();
                _serviceStopSignal.Wait();
            } catch (Exception ex) when (ex is ObjectDisposedException || ex is SemaphoreFullException) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
                return false;
            }
            return base.Finalize();
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

        private void StartService() {
            Uri baseAddress = new Uri(Configuration.Address.Value);

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(CommunicationService), baseAddress)) {
                NetTcpBinding binding = CreateNetTcpBinding();
                host.AddServiceEndpoint(typeof(IPluginService), binding, baseAddress);
                host.AddServiceEndpoint(typeof(IProjectService), binding, baseAddress);
                host.Description.Behaviors.Add(CreateServiceMetadataBehavior());
                host.Open();
                _serviceStopSignal.Wait();
                host.Close();
            }

            _serviceStopSignal = new SemaphoreSlim(0, 1);
        }
    }
}