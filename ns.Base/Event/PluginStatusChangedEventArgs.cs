using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class PluginStatusChangedEventArgs : EventArgs {
        private Plugin _plugin;
        private PluginStatus _status;

        public Plugin Plugin { get { return _plugin; } }
        public PluginStatus Status { get { return _status; } }

        public PluginStatusChangedEventArgs(Plugin plugin, PluginStatus status) {
            _plugin = plugin;
            _status = status;
        }
    }
}
