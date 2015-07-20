using ns.Base;
using ns.Base.Event;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core.Manager {
    public class DisplayManager : BaseManager {
        public event NodeCollectionChangedHandler ImageChangedEvent;
        public event EventHandler ClearEvent;

        public ProjectManager _projectManager;

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            _projectManager.OperationRemovedEvent += ProjectManagerOperationRemoved;
            return true;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(Node node) {
            if (node is ImageProperty && node.Parent is Tool && ((ImageProperty)node).IsOutput) {
                node.NodeChanged += ImageChangedEventHandle;
                base.Add(node);
            } else if (node is Operation)
                base.Add(node);
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Node node) {
            if (node is ImageProperty) {
                node.NodeChanged -= ImageChangedEventHandle;
                base.Remove(node);
            } else if (node is Operation)
                base.Remove(node);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear() {
            foreach(Node node in this.Nodes)
                node.NodeChanged -= ImageChangedEventHandle;
            this.Nodes.Clear();

            if (this.ClearEvent != null)
                this.ClearEvent(this, new EventArgs());
        }


        private void ImageChangedEventHandle(object sender, NodeChangedEventArgs e) {
            if (e.Name == "Value") {
                if (this.ImageChangedEvent != null) {
                    this.ImageChangedEvent(sender, new NodeCollectionChangedEventArgs(sender as ImageProperty));
                }
            }
        }

        private void ProjectManagerOperationRemoved(object sender, ChildCollectionChangedEventArgs e) {
            if(e.ChangedChilds.Count > 0)
                OnNodeRemoved(e.ChangedChilds[0]);
        }
    }
}
