using ns.Base;
using ns.Base.Event;
using ns.Base.Plugins;
using ns.Core;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für OperationNodeControl.xaml
    /// </summary>
    public partial class OperationNodeControl : UserControl {
        private Operation _operation;
        private LockedObservableCollection<ToolNodeControl> _toolControls;
        private GuiManager _guiManager;

        public delegate void ConfigNodeHandler(object sender, NodeSelectionChangedEventArgs e);

        public event ConfigNodeHandler ConfigNodeHandlerChanged;

        public OperationNodeControl(Operation operation) {
            InitializeComponent();
            DataContext = operation;
            _operation = operation;
            ContentList.Height = double.NaN;
            ContentToggleButton.IsChecked = true;
            _toolControls = new LockedObservableCollection<ToolNodeControl>();
            ContentList.ItemsSource = _toolControls;
            _operation.Childs.CollectionChanged += Operation_Childs_CollectionChanged;
            ContentList.SelectionChanged += ContentList_SelectionChanged;

            if (_guiManager == null)
                _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            _guiManager.SelectNode(operation);
        }

        private void ContentList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_guiManager == null)
                _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            if (ContentList.SelectedItem is ToolNodeControl && (ContentList.SelectedItem as ToolNodeControl).Tool != null) {
                _guiManager.SelectNode((ContentList.SelectedItem as ToolNodeControl).Tool);
            }
        }

        private void OnConfigNode(Node node) {
            ConfigNodeHandlerChanged?.Invoke(this, new NodeSelectionChangedEventArgs(node));
        }

        public void UpdateChildControls() {
            ContentList.Dispatcher.BeginInvoke(new Action(() => {
                _toolControls.Clear();
                List<ToolNodeControl> toolControls = new List<ToolNodeControl>();

                foreach (Node child in _operation.Childs) {
                    if (child is Tool) {
                        Tool tool = child as Tool;
                        ToolNodeControl toolNodeControl = new ToolNodeControl(tool);
                        toolNodeControl.ConfigNodeHandlerChanged += ToolNodeControl_ConfigNodeHandlerChanged;
                        toolControls.Add(toolNodeControl);
                    }
                }

                _toolControls.AddItems(toolControls);
            }));
        }

        private void ToolNodeControl_ConfigNodeHandlerChanged(object sender, NodeSelectionChangedEventArgs e) {
            OnConfigNode(e.SelectedNode);
        }

        private void Operation_Childs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateChildControls();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            ContentList.Height = double.NaN;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            ContentList.Height = ContentList.ActualHeight;
            this.ContentToggleButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                this.ContentList.Height = 0;
                this.ContentList.ApplyAnimationClock(Rectangle.HeightProperty, null);
                this.ContentToggleButton.IsEnabled = true;
            };
            ContentList.BeginAnimation(Rectangle.HeightProperty, animation);
        }
    }
}