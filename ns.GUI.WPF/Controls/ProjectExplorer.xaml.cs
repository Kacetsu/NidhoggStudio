using ns.Base;
using ns.Base.Event;
using ns.Communication.Client;
using ns.Communication.CommunicationModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl {
        //private GuiManager _guiManager;

        private Task _task;

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs e);

        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        public ProjectExplorer() {
            InitializeComponent();
            Loaded += HandleLoaded;
        }

        private void OnConfigNode(Node node) {
            ConfigNodeHandlerChanged?.Invoke(this, new NodeSelectionChangedEventArgs(node));
        }

        private void GenerateTree() {
            //if (_projectManager != null) {
            //    _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //    //_projectManager.Loading += ProjectManagerLoading;
            //}
            //if (_projectManager != null) {
            //    this.ContentGrid.Children.Clear();

            //    foreach (Operation operation in _projectManager.Configuration.Operations) {
            //        OperationNodeControl operationItem = new OperationNodeControl(operation);
            //        RowDefinition rowDefinition = new RowDefinition();
            //        rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            //        this.ContentGrid.RowDefinitions.Add(rowDefinition);
            //        Grid.SetRow(operationItem, this.ContentGrid.Children.Count);
            //        this.ContentGrid.Children.Add(operationItem);
            //        foreach (Tool tool in operation.Childs.Where(c => c is Tool)) {
            //            _guiManager.SelectNode(tool);
            //        }

            //        operationItem.UpdateChildControls();
            //        operationItem.ConfigNodeHandlerChanged += OperationItem_ConfigNodeHandlerChanged;
            //    }
            //}

            List<OperationCommunicationModel> operationModels = ClientCommunicationManager.ProjectService.GetProjectOperations();

            ContentGrid.Dispatcher.BeginInvoke(new Action(() => {
                foreach (OperationCommunicationModel operationModel in operationModels) {
                    OperationNodeControl operationItem = new OperationNodeControl(operationModel);
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
                    ContentGrid.RowDefinitions.Add(rowDefinition);
                    Grid.SetRow(operationItem, ContentGrid.Children.Count);
                    ContentGrid.Children.Add(operationItem);
                }

                if (ContentGrid.Children.Count > 0) {
                    OperationNodeControl operationControl = ContentGrid.Children[0] as OperationNodeControl;
                    if (operationControl != null) {
                        FrontendManager.SelectedModel = operationControl.Model;
                    }
                }
            }));
        }

        private void OperationItem_ConfigNodeHandlerChanged(object sender, NodeSelectionChangedEventArgs e) {
            OnConfigNode(e.SelectedNode);
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            //_projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //_guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(GuiManager))) as GuiManager;

            //if (_projectManager == null)
            //    throw new Exception("ProjectManager is NULL!");
            //_projectManager.Loaded += HandleProjectLoaded;
            _task = new Task(GenerateTree);
            _task.Start();
        }

        private void HandleProjectLoaded() {
            GenerateTree();
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