using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class ChildCollectionChangedEventArgs : EventArgs {
        private IReadOnlyCollection<Node> _changedChilds = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedChilds">The changed childs.</param>
        public ChildCollectionChangedEventArgs(IReadOnlyCollection<Node> changedChilds) {
            _changedChilds = changedChilds;
        }

        /// <summary>
        /// Gets the changed childs.
        /// </summary>
        /// <value>
        /// The changed childs.
        /// </value>
        public IReadOnlyCollection<Node> ChangedChilds {
            get { return _changedChilds; }
        }
    }
}