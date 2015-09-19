using ns.Base.Plugins;
using System.ComponentModel;

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
            _tool.PropertyChanged += HandlePropertyChanged;
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public override void Close() {
            base.Close();
            if (_tool != null) {
                _tool.PropertyChanged -= HandlePropertyChanged;
                _tool = null;
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name") {
                this.TextControl.Text = _tool.Name;
            }
        }

    }
}
