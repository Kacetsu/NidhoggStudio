using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core.Manager {
    public class ExtensionManager : PluginManager {

        private List<Plugin> _plugins = new List<Plugin>();

        public new List<Plugin> Plugins {
            get {
                return _plugins;
            }
        }

        public override bool Initialize() {
            foreach (Extension extension in _plugins) {
                extension.Initialize();
            }
            return true;
        }

        public override bool Finalize() {
            foreach (Extension extension in _plugins) {
                extension.Finalize();
            }
            return true;
        }

        public void RunAll() {
            foreach (Extension extension in _plugins) {
                if (extension.PreRun())
                    extension.Run();
                extension.PostRun();
            }
        }
    }
}
