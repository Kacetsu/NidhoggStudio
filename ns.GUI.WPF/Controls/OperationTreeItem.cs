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

        public OperationTreeItem(Operation operation) : base(operation) {
            _operation = operation;
            _operation.ChildCollectionChanged += HandleChildCollectionChanged;
            _operation.NodeChanged += HandlePropertyChanged;
        }

        private void HandlePropertyChanged(object sender, Base.Event.NodeChangedEventArgs e) {
            if (e.Value != null && !(e.Value is ns.Base.Plugins.Properties.Property)) {
                if (string.IsNullOrEmpty(e.Name) == false) {
                    if (e.Name == "Name") {
                        this.TextControl.Text = _operation.Name;
                    }
                }
            }
        }

        private void HandleChildCollectionChanged(object sender, Base.Event.ChildCollectionChangedEventArgs e) {
            this.UpdateChilds();
        }
    }
}
