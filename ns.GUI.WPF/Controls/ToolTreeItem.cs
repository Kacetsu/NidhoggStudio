using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF.Controls {
    public class ToolTreeItem : NodeTreeItem {

        private Tool _tool;

        /// <summary>
        /// Gets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        public Tool Tool {
            get { return _tool; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTreeItem"/> class.
        /// </summary>
        /// <param name="tool">The tool.</param>
        public ToolTreeItem(Tool tool)
            : base(tool) {
            _tool = tool;
            _tool.NodeChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            base.Close();
            if (_tool != null) {
                _tool.NodeChanged -= HandlePropertyChanged;
                _tool = null;
            }
        }

        private void HandlePropertyChanged(object sender, Base.Event.NodeChangedEventArgs e) {
            if (e.Value != null && !(e.Value is ns.Base.Plugins.Properties.Property)) {
                if (string.IsNullOrEmpty(e.Name) == false) {
                    if (e.Name == "Name") {
                        this.TextControl.Text = _tool.Name;
                    }
                }
            }
        }

    }
}
