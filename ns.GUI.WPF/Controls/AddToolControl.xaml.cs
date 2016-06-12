using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für AddToolControl.xaml
    /// </summary>
    public partial class AddToolControl : UserControl {
        private GuiManager _guiManager;
        private ProjectManager _projectManager;

        public AddToolControl() {
            InitializeComponent();
            this.Loaded += HandleLoaded;
        }

        private void AddToolToControl(Tool tool) {
            AddToolNodeControl nodeControl = new AddToolNodeControl(tool);
            int childCount = ToolGrid.Children.Count;
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            ToolGrid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(nodeControl, childCount);
            ToolGrid.Children.Add(nodeControl);
        }

        /// <summary>
        /// Handles the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleLoaded(object sender, RoutedEventArgs e) {
            PluginManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(GuiManager))) as GuiManager;
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;

            if (manager == null) {
                Base.Log.Trace.WriteLine("Could not find ToolManager instance in CoreSystem!", TraceEventType.Error);
                return;
            }

            if (_guiManager == null) {
                Base.Log.Trace.WriteLine("Could not find NodeSelectionManager instance in CoreSystem!", TraceEventType.Error);
                return;
            }

            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add("All");

            foreach (Tool tool in manager.Nodes.Where(n => n is Tool)) {
                if (!CategoryComboBox.Items.Contains(tool?.Category)) {
                    CategoryComboBox.Items.Add(tool?.Category);
                }
            }

            CategoryComboBox.SelectedIndex = 0;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            PluginManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;

            ToolGrid.Children.Clear();

            foreach (Tool tool in manager.Nodes.Where(n => n is Tool)) {
                if (tool.Category.Equals(CategoryComboBox.SelectedItem)) {
                    AddToolToControl(tool);
                } else if (CategoryComboBox.SelectedIndex == 0) {
                    AddToolToControl(tool);
                }
            }
        }
    }
}