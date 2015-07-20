using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            _operation.ChildCollectionChanged += HandleChildCollectionChanged;
            _operation.NodeChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            base.Close();
            if (_operation != null) {
                _operation.ChildCollectionChanged -= HandleChildCollectionChanged;
                _operation.NodeChanged -= HandlePropertyChanged;
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
        private void HandlePropertyChanged(object sender, Base.Event.NodeChangedEventArgs e) {
            if (e.Value != null && !(e.Value is ns.Base.Plugins.Properties.Property)) {
                if (string.IsNullOrEmpty(e.Name) == false) {
                    if (e.Name == "Name") {
                        this.TextControl.Text = _operation.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the child collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Base.Event.ChildCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void HandleChildCollectionChanged(object sender, Base.Event.ChildCollectionChangedEventArgs e) {
            this.UpdateChilds();
        }
    }
}
