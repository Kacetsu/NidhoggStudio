using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF.Controls {
    public class NodeTreeItem : TreeViewItem {

        private Node _node = null;
        private string _additionFormat = string.Empty;
        private TextBlock _textBlock;
        private Image _imageContainer;

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public Node Node {
            get { return _node; }
        }

        /// <summary>
        /// Gets the text control.
        /// </summary>
        /// <value>
        /// The text control.
        /// </value>
        public TextBlock TextControl {
            get { return _textBlock; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeTreeItem"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeTreeItem(Node node)
            : base() {
            this.AllowDrop = true;
            _textBlock = new TextBlock();
            this.Loaded += HandleLoaded;
            this.Unloaded += HandleUnloaded;
            this.DragOver += NodeTreeItem_DragOver;
            this.Drop += NodeTreeItem_Drop;
            this.MouseMove += NodeTreeItem_MouseMove;
            this.PreviewMouseDown += NodeTreeItem_PreviewMouseDown;
            this.Style = new Style(GetType(), this.FindResource(typeof(TreeViewItem)) as Style);
            _node = node;
            CreateHeaderPanel(node, string.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeTreeItem"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="additionalFormat">The additional format.</param>
        public NodeTreeItem(Node node, string additionalFormat) 
            : base() {
            this.AllowDrop = true;
            _textBlock = new TextBlock();
            this.Loaded += HandleLoaded;
            this.Unloaded += HandleUnloaded;
            this.DragOver += NodeTreeItem_DragOver;
            this.Drop += NodeTreeItem_Drop;
            this.MouseMove += NodeTreeItem_MouseMove;
            this.PreviewMouseDown += NodeTreeItem_PreviewMouseDown;
            this.Style = new Style(GetType(), this.FindResource(typeof(TreeViewItem)) as Style);
            _node = node;
            CreateHeaderPanel(node, additionalFormat);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() {
            if (_node != null) {
                _node.PropertyChanged -= HandlePropertyChanged;
                _node = null;
            }
        }

        /// <summary>
        /// Handles the unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleUnloaded(object sender, RoutedEventArgs e) {
            Close();
        }

        /// <summary>
        /// Creates the header panel.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="additionalFormat">The additional format.</param>
        private void CreateHeaderPanel(Node node, string additionalFormat) {
            string name = node.Name;
            string iconUrl = string.Empty;

            _additionFormat = additionalFormat;

            node.PropertyChanged += HandlePropertyChanged;

            if (node is StringProperty) {
                if (!string.IsNullOrEmpty(additionalFormat)) {
                    name = string.Format(additionalFormat, ((StringProperty)node).Value as string);
                } else
                    name = ((StringProperty)node).Value as string;
            } else if(node is Operation) {
                iconUrl = "/ns.GUI.WPF;component/Images/Operation.png";
            } else if (node is Tool) {
                iconUrl = "/ns.GUI.WPF;component/Images/Module.png";
                string description = ((Tool)node).Description;
                if (!string.IsNullOrEmpty(description))
                    this.ToolTip = description;
            }

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            if (!string.IsNullOrEmpty(iconUrl)) {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(iconUrl, UriKind.Relative);
                image.EndInit();

                _imageContainer = new Image();
                _imageContainer.Source = image;
                _imageContainer.Width = 24;
                _imageContainer.Height = 23;
                _imageContainer.Margin = new Thickness(2, 0, 2, 0);

                panel.Children.Add(_imageContainer);
            }

            _textBlock.Text = name;
            panel.Children.Add(_textBlock);

            this.Header = panel;
        }

        /// <summary>
        /// Updates the childs.
        /// </summary>
        internal void UpdateChilds() {
            this.Items.Clear();
            foreach (Plugin node in _node.Childs.Where(c => c is Plugin)) {
                NodeTreeItem item = null;
                if (node is Tool) {
                    item = new ToolTreeItem(node as Tool);
                } else if (node is Operation) {
                    item = new OperationTreeItem(node as Operation);
                }

                if (item != null)
                    this.Items.Add(item);
            }
        }

        /// <summary>
        /// Handles the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleLoaded(object sender, RoutedEventArgs e) {
            UpdateChilds();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name") {
                _textBlock.Text = string.Format(_additionFormat, _node.Name);
            } else if (e.PropertyName == "Value" && _node is StringProperty) {
                _textBlock.Text = string.Format(_additionFormat, ((StringProperty)_node).Value);
            } else if (e.PropertyName == "IsSelected") {
                this.IsSelected = _node.IsSelected;
            } else if (e.PropertyName == "Status") {
                Plugin plugin = sender as Plugin;
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() => {
                    switch (plugin.Status) {
                        case PluginStatus.Aborted:
                            this.BorderBrush = Brushes.Yellow;
                            break;
                        case PluginStatus.Failed:
                            this.BorderBrush = Brushes.Red;
                            break;
                        case PluginStatus.Finished:
                            this.BorderBrush = Brushes.Green;
                            break;
                        case PluginStatus.Started:
                            this.BorderBrush = Brushes.SteelBlue;
                            break;
                        case PluginStatus.Unknown:
                            this.BorderBrush = Brushes.Gray;
                            break;
                        default:
                            break;
                    }
                }));
            }
        }

        private Point _lasMouseDownPoistion;

        private void NodeTreeItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) {
                UIElement parent = GetParentElement();
                if(parent != null)
                    _lasMouseDownPoistion = e.GetPosition(parent);
            }
        }

        private void NodeTreeItem_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            try {
                UIElement parent = GetParentElement();
                if (parent == null || !(this.Node is Operation || this.Node is Tool)) return;
                if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                    Point currentPosition = e.GetPosition(parent);
                    if((Math.Abs(currentPosition.X - _lasMouseDownPoistion.X) > 10.0) || (Math.Abs(currentPosition.Y - _lasMouseDownPoistion.Y) > 10.0)) {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(this.Parent, this, DragDropEffects.Move);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        private void NodeTreeItem_Drop(object sender, DragEventArgs e) {
            int a = 0;
        }

        private void NodeTreeItem_DragOver(object sender, DragEventArgs e) {
            try {
                UIElement parent = GetParentElement();
                if (parent == null) return;
                Point currentPosition = e.GetPosition(parent);
                if ((Math.Abs(currentPosition.X - _lasMouseDownPoistion.X) > 10.0) || (Math.Abs(currentPosition.Y - _lasMouseDownPoistion.Y) > 10.0)) {
                    NodeTreeItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(item))
                        e.Effects = DragDropEffects.Move;
                    else
                        e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private NodeTreeItem GetNearestContainer(UIElement element) {
            NodeTreeItem container = element as NodeTreeItem;
            while((container == null) && (element != null)) {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as NodeTreeItem;
            }
            return container;
        }

        private bool CheckDropTarget(UIElement targetItem) {
            bool isEqual = false;
            if ((this != targetItem) && (targetItem is NodeTreeItem) && (((NodeTreeItem)targetItem).Node is Tool || ((NodeTreeItem)targetItem).Node is Operation))
                isEqual = true;
            return isEqual;
        }

        private UIElement GetParentElement() {
            return this.Parent as UIElement;
        }
    }
}
