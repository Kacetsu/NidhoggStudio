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
using ns.Base;
using System.ComponentModel;
using ns.Base.Plugins;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl {

        private ProjectManager _projectManager;
        private GuiManager _guiManager;

        public ProjectExplorer() {
            InitializeComponent();
            this.Loaded += HandleLoaded;
            this.ProjectTree.SelectedItemChanged += HandleSelectedItemChanged;
        }

        private void HandleSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            TreeView view = null;
            
            if (sender == this.ProjectTree)
                view = this.ProjectTree;

            if (view == null)
                throw new Exception("Project Explorer View is NULL!");

            TreeViewItem item = view.SelectedItem as TreeViewItem;
            if(item is OperationTreeItem)
                item.IsExpanded = true;

            if (view.SelectedItem is OperationTreeItem) {
                OperationTreeItem selectedItem = view.SelectedItem as OperationTreeItem;
                if (selectedItem == null)
                    throw new Exception("OperationTreeItem is NULL!");
                _guiManager.SelectNode(selectedItem.Operation);
            } else if (view.SelectedItem is ToolTreeItem) {
                ToolTreeItem selectedItem = view.SelectedItem as ToolTreeItem;
                if (selectedItem == null)
                    throw new Exception("ToolTreeItem is NULL!");
                _guiManager.SelectNode(selectedItem.Tool);
            } else if(view.SelectedItem is NodeTreeItem) {
                NodeTreeItem selectedItem = view.SelectedItem as NodeTreeItem;
                if (selectedItem == null)
                    throw new Exception("NodeTreeItem is NULL!");
                _guiManager.SelectNode(selectedItem.Node);
            } else {
                _guiManager.SelectNode(null);
            }
        }

        private void GenerateTree() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            if (_projectManager != null) {

                this.ProjectTree.Items.Clear();

                NodeTreeItem projectItem = new NodeTreeItem(_projectManager.Configuration.Name, "Project '{0}'");
                this.ProjectTree.Items.Add(projectItem);

                foreach (Operation operation in _projectManager.Configuration.Operations) {
                    OperationTreeItem operationItem = new OperationTreeItem(operation);
                    operationItem.IsExpanded = true;
                    this.ProjectTree.Items.Add(operationItem);
                }
            }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this) == false) {
                _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
                _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

                if (_projectManager == null)
                    throw new Exception("ProjectManager is NULL!");

                _projectManager.OperationAddedEvent += HandleOperationCollectionChanged;
                _projectManager.OperationRemovedEvent += HandleOperationCollectionChanged;
                _projectManager.Loaded += HandleProjectLoaded;
                GenerateTree();
            }
        }

        private void HandleOperationCollectionChanged(object sender, Base.Event.ChildCollectionChangedEventArgs e) {
            this.GenerateTree();
        }

        private void HandleProjectLoaded() {
            this.GenerateTree();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if(sender == this.DeleteButton) {
                Node selectedNode = _guiManager.SelectedNode;
                _projectManager.Remove(selectedNode);
            }
        }
    }
}
