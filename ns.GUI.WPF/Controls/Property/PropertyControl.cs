using ns.Base;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF.Controls.Property {
    public class PropertyControl : UserControl, INotifyPropertyChanged {
        private static Brush DEFAULT_BACKGROUNDBRUSH = Brushes.White;
        private ns.Base.Plugins.Properties.Property _property;
        private bool _isConnectable = false;
        private ComboBox _selectionComboBox = null;
        private Brush _backgroundBrush;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public ns.Base.Plugins.Properties.Property Property {
            get { return _property; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connectable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connectable; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsConnectable {
            get { return _isConnectable; }
            set { _isConnectable = value; }
        }

        public Brush BackgroundBrush {
            get { return _backgroundBrush; }
            set {
                _backgroundBrush = value;
                OnPropertyChanged("BackgroundBrush");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyControl"/> class.
        /// </summary>
        public PropertyControl() : base() {
            this.MouseEnter += Control_MouseEnter;
            this.MouseLeave += Control_MouseLeave;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public PropertyControl(ns.Base.Plugins.Properties.Property property)
            : base() {
            _property = property;
            this.MouseEnter += Control_MouseEnter;
            this.MouseLeave += Control_MouseLeave;
        }

        /// <summary>
        /// Connects the clicked.
        /// </summary>
        /// <param name="control">The control.</param>
        public void ConnectClicked(Control control, Image imageContainer) {
            UIElement parentControl = control.Parent as UIElement;
            BitmapImage image = new BitmapImage();

            if (control.Visibility == System.Windows.Visibility.Visible) {
                control.Visibility = System.Windows.Visibility.Collapsed;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Disconnect.png", UriKind.Relative);
                image.EndInit();
                CreateSelection(control, parentControl);
            } else {
                control.Visibility = System.Windows.Visibility.Visible;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Connect.png", UriKind.Relative);
                image.EndInit();
                DestroySelection(parentControl);
            }

            imageContainer.Source = image;
        }

        /// <summary>
        /// Connects the clicked.
        /// </summary>
        /// <param name="panel">The panel.</param>
        public void ConnectClicked(Panel panel, Image imageContainer) {
            UIElement parentControl = panel.Parent as UIElement;
            BitmapImage image = new BitmapImage();

            if (panel.Visibility == System.Windows.Visibility.Visible) {
                panel.Visibility = System.Windows.Visibility.Collapsed;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Disconnect.png", UriKind.Relative);
                image.EndInit();
                CreateSelection(panel, parentControl);
            } else {
                panel.Visibility = System.Windows.Visibility.Visible;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Connect.png", UriKind.Relative);
                image.EndInit();
                DestroySelection(parentControl);
            }

            imageContainer.Source = image;
        }

        protected void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Creates the selection.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="parentControl">The parent control.</param>
        private void CreateSelection(UIElement control, UIElement parentControl) {
            _selectionComboBox = new ComboBox();
            _selectionComboBox.Name = "SelectionComboBox";
            _selectionComboBox.SelectionChanged += SelectionComboBox_SelectionChanged;
            UpdateConnectablePropertyList();
            int column = Grid.GetColumn(control);
            int row = Grid.GetRow(control);
            int columnSpan = Grid.GetColumnSpan(control);
            int rowSpan = Grid.GetRowSpan(control);

            Grid.SetColumn(_selectionComboBox, column);
            Grid.SetRow(_selectionComboBox, row);
            Grid.SetColumnSpan(_selectionComboBox, columnSpan);
            Grid.SetRowSpan(_selectionComboBox, rowSpan);

            if (parentControl is Grid) {
                Grid gridParent = parentControl as Grid;
                gridParent.Children.Add(_selectionComboBox);
            }
        }

        private void SelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            ns.Base.Plugins.Properties.Property targetProperty = c.SelectedItem as ns.Base.Plugins.Properties.Property;
            if (targetProperty != null) {
                this.Property.Connect(targetProperty);
            } else if(c.SelectedItem is Operation) {
                this.Property.Connect(((Operation)c.SelectedItem).UID);
            }
        }

        /// <summary>
        /// Destroys the selection.
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        private void DestroySelection(UIElement parentControl) {
            Property.Unconnect();
            if (parentControl is Grid) {
                Grid gridParent = parentControl as Grid;
                if (gridParent.Children.Contains(_selectionComboBox))
                    gridParent.Children.Remove(_selectionComboBox);
                _selectionComboBox.SelectionChanged -= SelectionComboBox_SelectionChanged;
                _selectionComboBox = null;
            }
        }

        /// <summary>
        /// Updates the connectable property list.
        /// </summary>
        private void UpdateConnectablePropertyList() {
            if (this.Property is OperationSelectionProperty) {
                ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
                List<Operation> operations = (List<Operation>)projectManager.Configuration.Operations.FindAll(o => o.UID != this.Property.Parent.UID);
                _selectionComboBox.ItemsSource = operations;
                _selectionComboBox.DisplayMemberPath = "Name";

                if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                    Operation targetOperation = projectManager.Configuration.Operations.Find(p => p.UID == Property.ConnectedToUID);
                    if (targetOperation != null)
                        _selectionComboBox.SelectedItem = targetOperation;
                }

            } else {
                PropertyManager propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
                List<ns.Base.Plugins.Properties.Property> properties = propertyManager.GetConnectableProperties(this.Property);
                _selectionComboBox.ItemsSource = properties;
                _selectionComboBox.DisplayMemberPath = "TreeName";

                if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                    ns.Base.Plugins.Properties.Property targetProperty = properties.Find(p => p.UID == Property.ConnectedToUID);
                    if (targetProperty != null)
                        _selectionComboBox.SelectedItem = targetProperty;
                }
            }
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e) {
            BackgroundBrush = DEFAULT_BACKGROUNDBRUSH;
        }

        private void Control_MouseEnter(object sender, MouseEventArgs e) {
            BackgroundBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        }
    }
}
