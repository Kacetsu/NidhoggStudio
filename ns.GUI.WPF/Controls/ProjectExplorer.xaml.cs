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
    /// Logic for <see cref="ProjectExplorer"/> frontend.
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    /// <seealso cref="IDisposable" />
    public partial class ProjectExplorer : UserControl, IDisposable {
        private IEnumerable<OperationModel> _operationModels;
        private Task _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectExplorer"/> class.
        /// </summary>
        public ProjectExplorer() {
            InitializeComponent();
            Loaded += HandleLoaded;
        }

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs<object> e);

        /// <summary>
        /// Occurs when [configuration node handler changed].
        /// </summary>
        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
            ClientCommunicationManager.ProjectService.Callback.ToolRemoved -= ProjectServiceCallback_ToolRemoved;
            ClientCommunicationManager.ProjectService.Callback.ToolRemoved += ProjectServiceCallback_ToolRemoved;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _task = new Task(GenerateTree);
            _task.Start();
        }

        private void HandleProjectLoaded() {
            GenerateTree();
        }

        private void OnConfigNode(object node) {
            ConfigNodeHandlerChanged?.Invoke(this, new NodeSelectionChangedEventArgs<object>(node));
        }

        private void ProjectServiceCallback_ToolAdded(object sender, Communication.Events.CollectionChangedEventArgs e) {
            Guid parentId = e.NewObjects.Count() > 0 ? (e.NewObjects.ElementAt(0) as ToolModel).ParentId : Guid.Empty;
            foreach (UIElement element in ContentGrid.Children) {
                OperationNodeControl operationControl = element as OperationNodeControl;
                if (operationControl == null || !(operationControl.Model as IPluginModel).Id.Equals(parentId)) continue;
                operationControl.AddChildControls(e.NewObjects as IEnumerable<ToolModel>);
                break;
            }
        }

        private void ProjectServiceCallback_ToolRemoved(object sender, Communication.Events.CollectionChangedEventArgs e) {
            Guid parentId = e.NewObjects.Count() > 0 ? (e.NewObjects.ElementAt(0) as ToolModel).ParentId : Guid.Empty;
            foreach (UIElement element in ContentGrid.Children) {
                OperationNodeControl operationControl = element as OperationNodeControl;
                if (operationControl == null || !(operationControl.Model as IPluginModel).Id.Equals(parentId)) continue;
                operationControl.RemoveChildControls(e.NewObjects as IEnumerable<ToolModel>);
                FrontendManager.SelectedModel = operationControl.Model;
                break;
            }
        }
    }
}