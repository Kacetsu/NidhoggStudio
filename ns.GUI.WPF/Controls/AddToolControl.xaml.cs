using ns.Base;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            ToolManager manager = CoreSystem.Managers.Find(m => m.Name.Contains("ToolManager")) as ToolManager;
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;

            if (manager == null) {
                Trace.WriteLine("Could not find ToolManager instance in CoreSystem!", LogCategory.Error);
                return;
            }

            if (_guiManager == null) {
                Trace.WriteLine("Could not find NodeSelectionManager instance in CoreSystem!", LogCategory.Error);
                return;
            }

            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add("All");

            foreach (Node node in manager.Plugins) {
                if (node is Tool) {
                    Tool tool = node as Tool;
                    if (!CategoryComboBox.Items.Contains(tool.Category)) {
                        CategoryComboBox.Items.Add(tool.Category);
                    }
                }
            }

            CategoryComboBox.SelectedIndex = 0;
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ToolManager manager = CoreSystem.Managers.Find(m => m.Name.Contains("ToolManager")) as ToolManager;

            ToolGrid.Children.Clear();

            foreach(Node node in manager.Plugins) {
                if(node is Tool) {
                    Tool tool = node as Tool;
                    if (tool.Category.Equals(CategoryComboBox.SelectedItem)) {
                        AddToolToControl(tool);
                    }else if(CategoryComboBox.SelectedIndex == 0) {
                        AddToolToControl(tool);
                    }
                }
            }
        }
    }
}
