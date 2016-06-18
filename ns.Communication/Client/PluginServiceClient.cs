using ns.Communication.CommunicationModels;
using ns.Communication.Services;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class PluginServiceClient : GenericServiceClient<IPluginService>, IPluginService {

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        public PluginServiceClient(EndpointAddress endpoint, Binding binding) : base(endpoint, binding) {
        }

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns></returns>
        public List<PluginCommunicationModel> GetAvailablePlugins() => Channel?.GetAvailablePlugins();

        /// <summary>
        /// Gets the available tools.
        /// </summary>
        /// <returns></returns>
        public List<ToolCommunicationModel> GetAvailableTools() => Channel?.GetAvailableTools();
    }
}