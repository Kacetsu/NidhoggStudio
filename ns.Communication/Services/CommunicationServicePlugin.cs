using ns.Base.Plugins;
using ns.Communication.CommunicationModels;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ns.Communication.Services {

    public partial class CommunicationService : IPluginService {
        private ProjectManager _projectManager;
        private PluginManager _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationService"/> class.
        /// </summary>
        public CommunicationService() {
            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
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