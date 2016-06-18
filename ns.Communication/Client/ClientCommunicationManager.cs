using ns.Base.Manager;
using ns.Communication.Configuration;
using System;
using System.ServiceModel;

namespace ns.Communication.Client {

    public class ClientCommunicationManager : GenericConfigurationManager<CommunicationConfiguration>, IGenericConfigurationManager<CommunicationConfiguration> {
        private static Lazy<ClientCommunicationManager> _lazyInstance = new Lazy<ClientCommunicationManager>(() => new ClientCommunicationManager());

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
        /// Gets the project service.
        /// </summary>
        /// <value>
        /// The project service.
        /// </value>
        public static ProjectServiceClient ProjectService { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCommunicationManager"/> class.
        /// </summary>
        public ClientCommunicationManager() : base() {
            Configuration = new CommunicationConfiguration();
        }

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            base.Initialize();
            try {
                NetTcpBinding binding = new NetTcpBinding();
                binding.MaxReceivedMessageSize = Configuration.MaxReceivedMessageSize;

                PluginService = new PluginServiceClient(new EndpointAddress(Configuration.Address.Value), binding);
                ProjectService = new ProjectServiceClient(new EndpointAddress(Configuration.Address.Value), binding);
            } catch (Exception) {
                throw;
            }

            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Finalize() {
            PluginService?.Dispose();

            return base.Finalize();
        }
    }
}