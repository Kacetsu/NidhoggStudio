using ns.Base.Manager;
using ns.Communication.Configuration;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;

namespace ns.Communication.Client {

    public class ClientCommunicationManager : GenericConfigurationManager<CommunicationConfiguration>, IGenericConfigurationManager<CommunicationConfiguration>, ICommunicationManager {
        private static Lazy<ClientCommunicationManager> _lazyInstance = new Lazy<ClientCommunicationManager>(() => new ClientCommunicationManager());

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommunicationManager"/> class.
        /// </summary>
        public ClientCommunicationManager() : base() {
        }

        /// <summary>
        /// Gets the data storage service.
        /// </summary>
        /// <value>
        /// The data storage service.
        /// </value>
        public static DataStorageServiceClient DataStorageService { get; private set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ClientCommunicationManager Instance { get; } = _lazyInstance.Value;

        /// <summary>
        /// Gets the plugin service.
        /// </summary>
        /// <value>
        /// The plugin service.
        /// </value>
        public static PluginServiceClient PluginService { get; private set; }

        /// <summary>
        /// Gets the processor service.
        /// </summary>
        /// <value>
        /// The processor service.
        /// </value>
        public static ProcessorServiceClient ProcessorService { get; private set; }

        /// <summary>
        /// Gets the project service.
        /// </summary>
        /// <value>
        /// The project service.
        /// </value>
        public static ProjectServiceClient ProjectService { get; private set; }

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
            PluginService?.Dispose();
            ProjectService?.Dispose();
            ProcessorService?.Dispose();
            DataStorageService?.Dispose();

            base.Close();
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect() {
            if (IsConnected) return;
            Configuration = new CommunicationConfiguration();
            NetTcpBinding binding = new NetTcpBinding();
            binding.MaxReceivedMessageSize = Configuration.MaxReceivedMessageSize;

            PluginService = new PluginServiceClient(new EndpointAddress(Configuration.PluginServiceAddress), binding);
            ProjectService = new ProjectServiceClient(new EndpointAddress(Configuration.ProjectServiceAddress), binding, new ProjectServiceCallbacks());
            ProcessorService = new ProcessorServiceClient(new EndpointAddress(Configuration.ProcessorServiceAddress), binding, new ProcessorServiceCallbacks());
            DataStorageService = new DataStorageServiceClient(new EndpointAddress(Configuration.DataStorageServiceAddress), binding, new DataStorageServiceCallbacks());
            IsConnected = true;
        }
    }
}