using ns.Base.Plugins;
using ns.Core;
using ns.Core.Configuration;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication {

    public class CommunicationService : ICommunicationService {
        private ProjectManager _projectManager;
        private PluginManager _pluginManager;

        public CommunicationService() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
        }

        public List<Plugin> GetPlugins() => _pluginManager?.Nodes;

        public ProjectConfiguration GetProjectConfiguration() => _projectManager?.Configuration;
    }
}