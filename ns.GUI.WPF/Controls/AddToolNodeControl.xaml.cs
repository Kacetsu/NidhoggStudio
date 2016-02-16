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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für AddToolNodeControl.xaml
    /// </summary>
    public partial class AddToolNodeControl : UserControl {
        private Plugin _plugin;

        public AddToolNodeControl() {
            InitializeComponent();
        }

        public AddToolNodeControl(Plugin plugin) {
            InitializeComponent();
            DescriptionTextBlock.Height = 0;
            DescriptionToggleButton.IsChecked = false;
            DataContext = plugin;
            _plugin = plugin;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = double.NaN;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = DescriptionTextBlock.ActualHeight;
            this.DescriptionToggleButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                this.DescriptionTextBlock.Height = 0;
                this.DescriptionTextBlock.ApplyAnimationClock(Rectangle.HeightProperty, null);
                this.DescriptionToggleButton.IsEnabled = true;
            };
            DescriptionTextBlock.BeginAnimation(Rectangle.HeightProperty, animation);
        }

        //private void AddTool() {
        //    ListBox view = null;

        //    if (this.AnyTabs.SelectedItem == this.MainToolsTab) {

        //        TabItem selectedTab = this.ToolTabs.SelectedItem as TabItem;
        //        view = selectedTab.Content as ListBox;

        //        if (view != null) {
        //            Tool tool = view.SelectedItem as Tool;

        //            if (tool == null)
        //                return;

        //            if (_guiManager.SelectedNode != null && _guiManager.SelectedNode is Tool) {
        //                _guiManager.SelectNode(_guiManager.SelectedNode.Parent);
        //            }

        //            if (_guiManager.SelectedNode == null) {
        //                ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
        //                if (manager.Configuration.Operations.Count > 0)
        //                    _guiManager.SelectNode(manager.Configuration.Operations[0]);
        //            }

        //            if (_guiManager.SelectedNode != null && _guiManager.SelectedNode is Operation) {
        //                _lastAddedNode = tool.Clone() as Node;
        //                _projectManager.Add(_lastAddedNode, _guiManager.SelectedNode);
        //            }
        //        }
        //    } else if (this.AnyTabs.SelectedItem == this.MainOperationsTabs) {
        //        view = this.ListViewAllOperations;

        //        Operation operation = view.SelectedItem as Operation;

        //        if (operation == null)
        //            return;

        //        _lastAddedNode = operation.Clone() as Operation;
        //        _projectManager.Add(_lastAddedNode);
        //        _guiManager.SelectNode(_lastAddedNode);
        //    }
        //}

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            GuiManager guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;

            if (_plugin is Tool) {
                if(guiManager.SelectedNode == null) {
                    if (projectManager.Configuration.Operations.Count > 0)
                        guiManager.SelectNode(projectManager.Configuration.Operations[0]);
                }else if(guiManager.SelectedNode is Tool) {
                    guiManager.SelectNode(guiManager.SelectedNode.Parent);
                }

                Plugin clone = _plugin.Clone() as Plugin;
                projectManager.Add(clone, guiManager.SelectedNode);

                if (clone is Tool)
                    guiManager.SelectNode(clone);

            } else if(_plugin is Operation) {
                throw new NotSupportedException("Operations are not supperted yet!");
            }
            

        }
    }
}
