using ns.Communication.CommunicationModels;
using System;

namespace ns.GUI.WPF.Events {

    public class NodeSelectionChangedEventArgs<T> : EventArgs where T : class {

        /// <summary>
        /// Gets the selected node.
        /// </summary>
        /// <value>
        /// The selected node.
        /// </value>
        public T SelectedNode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeSelectionChangedEventArgs(T node) {
            SelectedNode = node;
        }
    }
}