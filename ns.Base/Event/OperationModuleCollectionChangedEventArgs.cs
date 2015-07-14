using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class OperationToolCollectionChangedEventArgs : EventArgs {

        private List<Tool> _addedTools = null;

        public List<Tool> AddedTools {
            get { return _addedTools; }
        }

        public OperationToolCollectionChangedEventArgs(List<Tool> addedTools) {
            _addedTools = addedTools;
        }
    }
}
