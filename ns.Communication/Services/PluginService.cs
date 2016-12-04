using ns.Base.Plugins;
using ns.Communication.Models;
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
            _pluginManager = CoreSystem.Instance.Plugins;
        }

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns></returns>
        public List<PluginModel> GetAvailablePlugins() {
            List<PluginModel> result = new List<PluginModel>();

            foreach (Plugin plugin in _pluginManager.Items.Values) {
                result.Add(new PluginModel(plugin));
            }

            return result;
        }

        /// <summary>
        /// Gets the available tools.
        /// </summary>
        /// <returns></returns>
        public List<ToolModel> GetAvailableTools() {
            List<ToolModel> result = new List<ToolModel>();

            foreach (Tool tool in _pluginManager.Items.Values.Where(t => t is Tool)) {
                result.Add(new ToolModel(tool));
            }

            return result;
        }
    }
}