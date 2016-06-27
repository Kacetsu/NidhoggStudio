using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for DevicePropertyControl.xaml
    /// </summary>
    public partial class DevicePropertyControl : PropertyControl<DeviceProperty> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePropertyControl"/> class.
        /// </summary>
        public DevicePropertyControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicePropertyControl"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="isConnectable">if set to <c>true</c> [is connectable].</param>
        public DevicePropertyControl(DeviceProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = this;

            SelectionBox.ItemsSource = _property.Value;
            SelectionBox.DisplayMemberPath = "DisplayName";

            Device device = null;
            if (_property.Value == null) {
                if ((device = _property.Value.Find(d => d.Name.Contains("ImageFileDevice")) as Device) != null)
                    SelectionBox.SelectedItem = device;
            } else {
                device = _property.SelectedItem as Device;
                SelectionBox.SelectedItem = device;
            }

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(SelectionBox as Control, ConnectImage);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(SelectionBox as Control, ConnectImage);
            }
        }

        private void SelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (_property.SelectedItem != SelectionBox.SelectedItem as Device)
                _property.SelectedItem = SelectionBox.SelectedItem as Device;
        }
    }
}