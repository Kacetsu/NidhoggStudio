using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class ChildCollectionChangedEventArgs : EventArgs {

        private List<Node> _changedChilds = null;

        /// <summary>
        /// Gets the changed childs.
        /// </summary>
        /// <value>
        /// The changed childs.
        /// </value>
        public List<Node> ChangedChilds {
            get { return _changedChilds; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedChilds">The changed childs.</param>
        public ChildCollectionChangedEventArgs(List<Node> changedChilds) {
            _changedChilds = changedChilds;
        }
    }
}
