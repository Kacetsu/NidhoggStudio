using ns.Communication.Client;
using ns.Communication.Models;
using ns.GUI.WPF.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für ProjectExplorer.xaml
    /// </summary>
    public partial class ProjectExplorer : UserControl, IDisposable {
        //private GuiManager _guiManager;

        private IEnumerable<OperationModel> _operationModels;
        private Task _task;

        public ProjectExplorer() {
            InitializeComponent();
            Loaded += HandleLoaded;
        }

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs<object> e);

        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                _task?.Dispose();
            }
        }

        private void GenerateTree() {
            OperationModel[] operationModels = ClientCommunicationManager.ProjectService.GetOperations();

            _operationModels = operationModels;

            ContentGrid.Dispatcher.BeginInvoke(new Action(() => {
                foreach (OperationModel operationModel in operationModels) {
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
                        FrontendManager.SelectedModel = operationControl.Model as IPluginModel;
                    }
                }
            }));

            ClientCommunicationManager.ProjectService.Callback.ToolAdded -= ProjectServiceCallback_ToolAdded;
            ClientCommunicationManager.ProjectService.Callback.ToolAdded += ProjectServiceCallback_ToolAdded;
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

        private void OnConfigNode(object node) {
            ConfigNodeHandlerChanged?.Invoke(this, new NodeSelectionChangedEventArgs<object>(node));
        }

        private void ProjectManagerLoading() {
            //foreach (NodeTreeItem item in this.ProjectTree.Items) {
            //    if (item is OperationTreeItem) {
            //        ((OperationTreeItem)item).Close();
            //    }
            //}
            //this.ProjectTree.Items.Clear();
        }

        private void ProjectServiceCallback_ToolAdded(object sender, Communication.Events.CollectionChangedEventArgs e) {
            string parentUID = e.NewObjects.Count() > 0 ? (e.NewObjects.ElementAt(0) as ToolModel).ParentUID : string.Empty;
            foreach (UIElement element in ContentGrid.Children) {
                OperationNodeControl operationControl = element as OperationNodeControl;
                if (operationControl == null || !(operationControl.Model as IPluginModel).UID.Equals(parentUID)) continue;
                operationControl.UpdateChildControls(e.NewObjects as IEnumerable<ToolModel>);
                break;
            }
        }
    }
}