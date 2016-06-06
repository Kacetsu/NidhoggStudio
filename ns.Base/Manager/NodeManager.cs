using ns.Base.Event;
using System;
using System.Collections.Generic;

namespace ns.Base.Manager {

    public class NodeManager<T> : BaseManager where T : Node {

        public delegate void NodeCollectionChangedHandler(object sender, NodeCollectionChangedEventArgs e);

        /// <summary>
        /// Occurs when [node added event].
        /// </summary>
        public event NodeCollectionChangedHandler NodeAddedEvent;

        /// <summary>
        /// Occurs when [node removed event].
        /// </summary>
        public event NodeCollectionChangedHandler NodeRemovedEvent;

        public NodeManager() {
            Nodes = new List<T>();
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public List<T> Nodes { get; }

        /// <summary>
        /// Called when [node added].
        /// </summary>
        /// <param name="node">The node.</param>
        public void OnNodeAdded(T node) {
            NodeAddedEvent?.Invoke(this, new NodeCollectionChangedEventArgs(node));
        }

        /// <summary>
        /// Called when [node removed].
        /// </summary>
        /// <param name="node">The node.</param>
        public void OnNodeRemoved(T node) {
            NodeRemovedEvent?.Invoke(this, new NodeCollectionChangedEventArgs(node));
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Add(T node) {
            if (!Nodes.Contains(node)) {
                Nodes.Add(node);
                OnNodeAdded(node);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public virtual void AddRange(List<T> nodes) {
            foreach (T node in nodes) {
                Add(node);
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Remove(T node) {
            if (Nodes.Contains(node)) {
                Nodes.Remove(node);
                OnNodeRemoved(node);
            }
        }

        /// <summary>
        /// Called when [selection changed].
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void OnSelectionChanged(T selectedObject) {
            throw new NotImplementedException();
        }
    }
}