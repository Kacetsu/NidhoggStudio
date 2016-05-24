using ns.Base;
using ns.Base.Event;
using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaktionslogik für ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl {
        private ProjectManager _projectManager;
        private GuiManager _guiManager;

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs e);
        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        public ProjectExplorer() {
            InitializeComponent();
            this.Loaded += HandleLoaded;
        }

        private void OnConfigNode(Node node) {
            if (ConfigNodeHandlerChanged != null)
                ConfigNodeHandlerChanged(this, new NodeSelectionChangedEventArgs(node));
        }

        private void GenerateTree() {
            if (_projectManager != null) {
                _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
                _projectManager.Loading += ProjectManagerLoading;
            }
            if (_projectManager != null) {

                this.ContentGrid.Children.Clear();

                foreach (Operation operation in _projectManager.Configuration.Operations) {
                    OperationNodeControl operationItem = new OperationNodeControl(operation);
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
                    this.ContentGrid.RowDefinitions.Add(rowDefinition);
                    Grid.SetRow(operationItem, this.ContentGrid.Children.Count);
                    this.ContentGrid.Children.Add(operationItem);
                    foreach (Tool tool in operation.Childs.Where(c => c is Tool)) {
                        _guiManager.SelectNode(tool);
                    }

                    operationItem.UpdateChildControls();
                    operationItem.ConfigNodeHandlerChanged += OperationItem_ConfigNodeHandlerChanged;
                }
            }
        }

        private void OperationItem_ConfigNodeHandlerChanged(object sender, NodeSelectionChangedEventArgs e) {
            OnConfigNode(e.SelectedNode);
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

        private void ProjectManagerLoading() {
            //foreach (NodeTreeItem item in this.ProjectTree.Items) {
            //    if (item is OperationTreeItem) {
            //        ((OperationTreeItem)item).Close();
            //    }
            //}
            //this.ProjectTree.Items.Clear();
        }

    }
}
