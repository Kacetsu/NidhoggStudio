using ns.Base.Plugins;
using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class OperationToolCollectionChangedEventArgs : EventArgs {
        private IReadOnlyCollection<Tool> _addedTools = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationToolCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="addedTools">The added tools.</param>
        public OperationToolCollectionChangedEventArgs(IReadOnlyCollection<Tool> addedTools) {
            _addedTools = addedTools;
        }

        /// <summary>
        /// Gets the added tools.
        /// </summary>
        /// <value>
        /// The added tools.
        /// </value>
        public IReadOnlyCollection<Tool> AddedTools {
            get { return _addedTools; }
        }
    }
}