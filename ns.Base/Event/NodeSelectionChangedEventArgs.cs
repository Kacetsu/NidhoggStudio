using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class NodeSelectionChangedEventArgs : EventArgs {

        private Node _node;

        public Node SelectedNode {
            get { return _node; }
        }

        public NodeSelectionChangedEventArgs(Node node) {
            _node = node;
        }
    }
}
