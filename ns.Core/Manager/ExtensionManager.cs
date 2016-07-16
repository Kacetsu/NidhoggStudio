﻿using ns.Base.Manager;
using ns.Base.Plugins;

namespace ns.Core.Manager {

    public class ExtensionManager : NodeManager<Extension>, INodeManager<Extension> {

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionManager"/> class.
        /// </summary>
        public ExtensionManager() {
            foreach (Extension extension in Nodes) {
                extension.Initialize();
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            foreach (Extension extension in Nodes) {
                extension.Close();
            }
        }

        /// <summary>
        /// Runs all.
        /// </summary>
        public void RunAll() {
            foreach (Extension extension in Nodes) {
                if (extension.TryPreRun())
                    extension.TryRun();
                extension.TryPostRun();
            }
        }
    }
}