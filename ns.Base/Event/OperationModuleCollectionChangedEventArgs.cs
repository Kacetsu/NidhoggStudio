using ns.Base.Plugins;
using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class OperationToolCollectionChangedEventArgs : EventArgs {
        private List<Tool> _addedTools = null;

        public OperationToolCollectionChangedEventArgs(List<Tool> addedTools) {
            _addedTools = addedTools;
        }

        public List<Tool> AddedTools {
            get { return _addedTools; }
        }
    }
}