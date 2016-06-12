using System.Collections.Generic;

namespace ns.Base.Manager {

    public interface INodeManager<T> : IManager where T : Node {
        List<T> Nodes { get; }

        /// <summary>
        /// Occurs when [node added event].
        /// </summary>
        event NodeManager<T>.NodeCollectionChangedHandler NodeAddedEvent;

        /// <summary>
        /// Occurs when [node removed event].
        /// </summary>
        event NodeManager<T>.NodeCollectionChangedHandler NodeRemovedEvent;

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Add(T node);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        void AddRange(List<T> nodes);

        /// <summary>
        /// Called when [node added].
        /// </summary>
        /// <param name="node">The node.</param>
        void OnNodeAdded(T node);

        /// <summary>
        /// Called when [node removed].
        /// </summary>
        /// <param name="node">The node.</param>
        void OnNodeRemoved(T node);

        /// <summary>
        /// Called when [selection changed].
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        void OnSelectionChanged(T selectedObject);

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Remove(T node);
    }
}