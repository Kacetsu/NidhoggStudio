using ns.Base.Log;
using ns.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Manager;

namespace ns.Core.Configuration {
    public class ProjectConfiguration : BaseConfiguration {

        private StringProperty _name;
        private LogConfiguration _logConfiguration;
        private List<Operation> _operations;
        private List<Device> _devices;
        private ProjectManager _projectManager;

        public bool Initialize() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            _name = new StringProperty("Name", "Unnamed Project");
            _operations = new List<Operation>();
            _devices = new List<Device>();

            if (_logConfiguration == null) {
                _logConfiguration = new LogConfiguration();
#if DEBUG
                _logConfiguration.Categories.Add(LogCategory.Debug.GetDescription());
#endif
                _logConfiguration.Categories.Add(LogCategory.Info.GetDescription());
                _logConfiguration.Categories.Add(LogCategory.Warning.GetDescription());
                _logConfiguration.Categories.Add(LogCategory.Error.GetDescription());
            }

            if (_projectManager.LoadLastUsedProject() == false) {
                _name = new StringProperty("Name", "Unnamed Project");
                _operations = new List<Operation>();
                _devices = new List<Device>();
                if (_operations.Count == 0) {
                    Operation operation = new Operation("Unnamed Operation");
                    _projectManager.Add(operation);
                }
            }

            return true;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public StringProperty Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the log configuration.
        /// </summary>
        /// <value>
        /// The log configuration.
        /// </value>
        public LogConfiguration LogConfiguration {
            get { return _logConfiguration; }
            set { _logConfiguration = value; }
        }

        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public List<Operation> Operations {
            get { return _operations; }
            set { _operations = value; }
        }

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public List<Device> Devices {
            get { return _devices; }
            set { _devices = value; }
        }
    }
}
