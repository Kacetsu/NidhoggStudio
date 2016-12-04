using System.Collections.Generic;

namespace ns.Base.Manager {

    public interface INodeManager<T> : IManager where T : Node {

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        void Add(T node);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        void AddRange(ICollection<T> nodes);

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