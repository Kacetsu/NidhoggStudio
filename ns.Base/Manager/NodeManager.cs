using ns.Base.Event;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ns.Base.Manager {

    public abstract class NodeManager<T> : BaseManager, INodeManager<T> where T : Node {

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeManager{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NodeManager([CallerMemberName] string name = null)
            : base(name) {
        }

        protected new delegate void EventHandler<NodeCollectionChangedEventArgs>(object sender, NodeCollectionChangedEventArgs e);

        /// <summary>
        /// Occurs when [node added event].
        /// </summary>
        protected event EventHandler<NodeCollectionChangedEventArgs> NodeAddedEvent;

        /// <summary>
        /// Occurs when [node removed event].
        /// </summary>
        protected event EventHandler<NodeCollectionChangedEventArgs> NodeRemovedEvent;

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Add(T node) {
            if (!Items.Values.Contains(node)) {
                Items.TryAdd(node.Id, node);
                OnNodeAdded(node);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public virtual void AddRange(ICollection<T> nodes) {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            foreach (T node in nodes) {
                Add(node);
            }
        }

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
        /// Called when [selection changed].
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void OnSelectionChanged(T selectedObject) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Remove(T node) {
            if (Items.ContainsKey(node.Id)) {
                Node outNode;
                Items.TryRemove(node.Id, out outNode);
                OnNodeRemoved(node);
            }
        }
    }
}