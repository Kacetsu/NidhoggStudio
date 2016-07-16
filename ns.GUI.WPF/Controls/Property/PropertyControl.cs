using ns.Base.Log;
using ns.Base.Plugins;
using ns.Communication.Client;
using ns.Communication.Models.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF.Controls.Property {

    public class PropertyControl<T> : UserControl, INotifyPropertyChanged where T : Base.Plugins.Properties.Property {
        protected T _property;
        private static Brush DEFAULT_BACKGROUNDBRUSH = Brushes.White;
        private Brush _backgroundBrush;
        private bool _isConnectable = false;
        private ComboBox _selectionComboBox = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyControl"/> class.
        /// </summary>
        public PropertyControl() : base() {
            MouseEnter += Control_MouseEnter;
            MouseLeave += Control_MouseLeave;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public PropertyControl(T property)
            : base() {
            _property = property;
            MouseEnter += Control_MouseEnter;
            MouseLeave += Control_MouseLeave;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the background brush.
        /// </summary>
        /// <value>
        /// The background brush.
        /// </value>
        public Brush BackgroundBrush {
            get { return _backgroundBrush; }
            set {
                _backgroundBrush = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName {
            get { return _property.Name; }
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

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public T Property {
            get { return _property; }
            protected set { _property = value; }
        }

        /// <summary>
        /// Connects the clicked.
        /// </summary>
        /// <param name="control">The control.</param>
        public void ConnectClicked(Control control, Image imageContainer) {
            UIElement parentControl = control.Parent as UIElement;
            BitmapImage image = new BitmapImage();

            if (control.Visibility == Visibility.Visible) {
                control.Visibility = Visibility.Collapsed;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Disconnect.png", UriKind.Relative);
                image.EndInit();
                CreateSelection(control, parentControl);
            } else {
                control.Visibility = Visibility.Visible;
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

            if (panel.Visibility == Visibility.Visible) {
                panel.Visibility = Visibility.Collapsed;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Disconnect.png", UriKind.Relative);
                image.EndInit();
                CreateSelection(panel, parentControl);
            } else {
                panel.Visibility = Visibility.Visible;
                image.BeginInit();
                image.UriSource = new Uri("/ns.GUI.WPF;component/Images/Connect.png", UriKind.Relative);
                image.EndInit();
                DestroySelection(parentControl);
            }

            imageContainer.Source = image;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Control_MouseEnter(object sender, MouseEventArgs e) {
            BackgroundBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        }

        private void Control_MouseLeave(object sender, MouseEventArgs e) {
            BackgroundBrush = DEFAULT_BACKGROUNDBRUSH;
        }

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

        private void SelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                ComboBox c = sender as ComboBox;
                PropertyModel sourceProperty = c.SelectedItem as PropertyModel;
                ClientCommunicationManager.ProjectService.ConnectProperties(Property.UID, sourceProperty.UID);
                Property.ConnectedUID = sourceProperty.UID;
            } catch (FaultException ex) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
            }
        }

        private void UpdateConnectablePropertyList() {
            try {
                PropertyModel[] propertyModels = ClientCommunicationManager.ProjectService.GetConnectableProperties(Property.UID);
                _selectionComboBox.ItemsSource = propertyModels;
                _selectionComboBox.DisplayMemberPath = nameof(PropertyModel.TreeName);

                if (string.IsNullOrEmpty(Property.ConnectedUID) && propertyModels.Length > 0) {
                    ClientCommunicationManager.ProjectService.ConnectProperties(Property.UID, propertyModels[0].UID);
                    Property.ConnectedUID = propertyModels[0].UID;
                    _selectionComboBox.SelectedItem = propertyModels[0];
                } else {
                    foreach (PropertyModel propertyModel in propertyModels) {
                        if (propertyModel.Property.UID.Equals(Property.ConnectedUID)) {
                            _selectionComboBox.SelectedItem = propertyModel;
                            break;
                        }
                    }
                }
            } catch (FaultException ex) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
            }
        }
    }
}