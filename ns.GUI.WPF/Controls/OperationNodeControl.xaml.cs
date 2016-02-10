using ns.Base;
using ns.Base.Event;
using ns.Base.Plugins;
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
            _operation.ChildCollectionChanged += _operation_ChildCollectionChanged;
        }

        private void OnConfigNode(Node node) {
            if (ConfigNodeHandlerChanged != null)
                ConfigNodeHandlerChanged(this, new NodeSelectionChangedEventArgs(node));
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

        private void _operation_ChildCollectionChanged(object sender, Base.Event.ChildCollectionChangedEventArgs e) {
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
