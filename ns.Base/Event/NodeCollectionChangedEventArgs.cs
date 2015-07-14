using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class NodeCollectionChangedEventArgs : EventArgs {
        private Node _node;

        public Node Node {
            get { return _node; }
        }

        public NodeCollectionChangedEventArgs(Node node) {
            _node = node;
        }
    }
}
