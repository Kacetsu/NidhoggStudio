using ns.Base;
using ns.Base.Event;
using ns.Base.Plugins;
using ns.Communication.CommunicationModels;
using ns.GUI.WPF.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für OperationNodeControl.xaml
    /// </summary>
    public partial class OperationNodeControl : UserControl, INodeControl {
        private OperationModel _operationModel;
        private LockedObservableCollection<ToolNodeControl> _toolControls;
        //private GuiManager _guiManager;

        public object Model { get { return _operationModel; } }

        public OperationNodeControl(OperationModel operationModel) {
            InitializeComponent();
            DataContext = operationModel;
            _operationModel = operationModel;
            ContentList.Height = double.NaN;
            ContentToggleButton.IsChecked = true;
            _toolControls = new LockedObservableCollection<ToolNodeControl>();
            ContentList.ItemsSource = _toolControls;
            //_operationModel.Childs.CollectionChanged += Operation_Childs_CollectionChanged;
            ContentList.SelectionChanged += ContentList_SelectionChanged;

            //if (_guiManager == null)
            //    _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            //_guiManager.SelectNode(operationModel);

            Loaded += OperationNodeControl_Loaded;
        }

        private void OperationNodeControl_Loaded(object sender, RoutedEventArgs e) {
            UpdateChildControls();
        }

        private void ContentList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if (_guiManager == null)
            //    _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            //if (ContentList.SelectedItem is ToolNodeControl && (ContentList.SelectedItem as ToolNodeControl).Tool != null) {
            //    _guiManager.SelectNode((ContentList.SelectedItem as ToolNodeControl).Tool);
            //}
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

        public void UpdateChildControls(IEnumerable<ToolModel> toolModels) {
            (Model as IOperationModel).ChildTools.AddRange(toolModels);
            UpdateChildControls();
        }

        private void Operation_Childs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateChildControls();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            ContentList.Height = double.NaN;
        }

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

        private void ConfigButton_Click(object sender, RoutedEventArgs e) {
            FrontendManager.OnNodeConfigurationClicked(this);
        }
    }
}