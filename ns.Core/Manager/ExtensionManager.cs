using ns.Base.Plugins;

namespace ns.Core.Manager {

    public class ExtensionManager : PluginManager {

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            foreach (Extension extension in Nodes) {
                extension.Initialize();
            }
            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Finalize() {
            foreach (Extension extension in Nodes) {
                extension.Finalize();
            }
            return true;
        }

        /// <summary>
        /// Runs all.
        /// </summary>
        public void RunAll() {
            foreach (Extension extension in Nodes) {
                if (extension.PreRun())
                    extension.Run();
                extension.PostRun();
            }
        }
    }
}