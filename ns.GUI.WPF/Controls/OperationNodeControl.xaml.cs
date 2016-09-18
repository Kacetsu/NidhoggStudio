using ns.Communication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Logic for <see cref="OperationNodeControl"/> frontend.
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="INodeControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class OperationNodeControl : UserControl, INodeControl {
        private IOperationModel _operationModel;
        private LockedObservableCollection<ToolNodeControl> _toolControls;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationNodeControl"/> class.
        /// </summary>
        /// <param name="operationModel">The operation model.</param>
        public OperationNodeControl(OperationModel operationModel) {
            InitializeComponent();
            DataContext = operationModel;
            _operationModel = operationModel;
            ContentList.Height = double.NaN;
            ContentToggleButton.IsChecked = true;
            _toolControls = new LockedObservableCollection<ToolNodeControl>();
            ContentList.ItemsSource = _toolControls;
            ContentList.SelectionChanged += ContentList_SelectionChanged;
            Loaded += OperationNodeControl_Loaded;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public IPluginModel Model { get { return _operationModel; } }

        /// <summary>
        /// Adds the child controls.
        /// </summary>
        /// <param name="toolModels">The tool models.</param>
        public void AddChildControls(IEnumerable<ToolModel> toolModels) {
            (Model as IOperationModel).ChildTools.AddRange(toolModels);
            UpdateChildControls();
        }

        /// <summary>
        /// Removes the child controls.
        /// </summary>
        /// <param name="toolModels">The tool models.</param>
        public void RemoveChildControls(IEnumerable<ToolModel> toolModels) {
            foreach (ToolModel toolModel in toolModels) {
                IOperationModel operationModel = Model as IOperationModel;
                ToolModel toolToRemove = operationModel.ChildTools.Find(t => t.Id.Equals(toolModel.Id));
                (Model as IOperationModel).ChildTools.Remove(toolToRemove);
            }
            UpdateChildControls();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e) => FrontendManager.OnNodeConfigurationClicked(this);

        private void ContentList_SelectionChanged(object sender, SelectionChangedEventArgs e) => FrontendManager.SelectedModel = (ContentList.SelectedItem as ToolNodeControl)?.Model;

        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            FrontendManager.SelectedModel = Model;
            ContentList.SelectedItem = null;
        }

        private void Operation_Childs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => UpdateChildControls();

        private void OperationNodeControl_Loaded(object sender, RoutedEventArgs e) => UpdateChildControls();

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) => ContentList.Height = double.NaN;

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            ContentList.Height = ContentList.ActualHeight;
            ContentToggleButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                ContentList.Height = 0;
                ContentList.ApplyAnimationClock(HeightProperty, null);
                ContentToggleButton.IsEnabled = true;
            };
            ContentList.BeginAnimation(HeightProperty, animation);
        }

        private void UpdateChildControls() {
            ContentList.Dispatcher.BeginInvoke(new Action(() => {
                _toolControls.Clear();
                foreach (ToolModel toolModel in _operationModel.ChildTools.Where(t => t is ToolModel)) {
                    ToolNodeControl toolNodeControl = new ToolNodeControl(toolModel);
                    _toolControls.Add(toolNodeControl);
                }
            }));
        }
    }
}