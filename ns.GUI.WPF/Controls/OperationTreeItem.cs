using ns.Base.Plugins;

namespace ns.GUI.WPF.Controls {

    public class OperationTreeItem : NodeTreeItem {
        private Operation _operation;

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public Operation Operation {
            get { return _operation; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationTreeItem"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationTreeItem(Operation operation) : base(operation) {
            _operation = operation;
            _operation.Childs.CollectionChanged += Operation_Childs_CollectionChanged;
            _operation.PropertyChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            base.Close();
            if (_operation != null) {
                _operation.Childs.CollectionChanged -= Operation_Childs_CollectionChanged;
                _operation.PropertyChanged -= HandlePropertyChanged;
                _operation = null;
            }

            foreach (ToolTreeItem item in this.Items)
                item.Close();
        }

        /// <summary>
        /// Handles the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Base.Event.NodeChangedEventArgs" /> instance containing the event data.</param>
        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name") {
                this.TextControl.Text = _operation.Name;
            }
        }

        private void Operation_Childs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateChilds();
        }
    }
}