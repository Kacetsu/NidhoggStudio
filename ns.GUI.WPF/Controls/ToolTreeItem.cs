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

        public ToolTreeItem(Tool tool)
            : base(tool) {
            _tool = tool;
            _tool.NodeChanged += HandlePropertyChanged;
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
