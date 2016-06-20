using ns.Base.Plugins;
using ns.Communication.CommunicationModels;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.Linq;

namespace ns.Communication.Services {

    public class PluginService : IPluginService {
        private PluginManager _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationService"/> class.
        /// </summary>
        public PluginService() {
            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
        }

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns></returns>
        public List<PluginCommunicationModel> GetAvailablePlugins() {
            List<PluginCommunicationModel> result = new List<PluginCommunicationModel>();

            foreach (Plugin plugin in _pluginManager.Nodes) {
                result.Add(new PluginCommunicationModel(plugin));
            }

            return result;
        }

        /// <summary>
        /// Gets the available tools.
        /// </summary>
        /// <returns></returns>
        public List<ToolCommunicationModel> GetAvailableTools() {
            List<ToolCommunicationModel> result = new List<ToolCommunicationModel>();

            foreach (Tool tool in _pluginManager.Nodes.Where(t => t is Tool)) {
                result.Add(new ToolCommunicationModel(tool));
            }

            return result;
        }
    }
}