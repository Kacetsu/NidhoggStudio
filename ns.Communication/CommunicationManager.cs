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

        private Task _pluginServiceTask;
        private Task _projectServiceTask;
        private SemaphoreSlim _pluginServiceStopSignal = new SemaphoreSlim(0, 1);
        private SemaphoreSlim _projectServiceStopSignal = new SemaphoreSlim(0, 1);
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
                _pluginServiceTask = new Task(StartPluginService);
                _projectServiceTask = new Task(StartProjectService);
                _pluginServiceTask.Start();
                _projectServiceTask.Start();
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
                _pluginServiceStopSignal.Release();
                _projectServiceStopSignal.Release();

                _pluginServiceStopSignal.Wait();
                _projectServiceStopSignal.Wait();
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

        private void StartPluginService() {
            Uri baseAddress = new Uri(Configuration.PluginServiceAddress);

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(PluginService), baseAddress)) {
                NetTcpBinding binding = CreateNetTcpBinding();
                host.AddServiceEndpoint(typeof(IPluginService), binding, baseAddress);
                host.Description.Behaviors.Add(CreateServiceMetadataBehavior());
                host.Open();
                _pluginServiceStopSignal.Wait();
                host.Close();
            }

            _pluginServiceStopSignal = new SemaphoreSlim(0, 1);
        }

        private void StartProjectService() {
            Uri baseAddress = new Uri(Configuration.ProjectServiceAddress);

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(ProjectService), baseAddress)) {
                NetTcpBinding binding = CreateNetTcpBinding();
                host.AddServiceEndpoint(typeof(IProjectService), binding, baseAddress);
                host.Description.Behaviors.Add(CreateServiceMetadataBehavior());
                host.Open();
                _projectServiceStopSignal.Wait();
                host.Close();
            }

            _projectServiceStopSignal = new SemaphoreSlim(0, 1);
        }
    }
}