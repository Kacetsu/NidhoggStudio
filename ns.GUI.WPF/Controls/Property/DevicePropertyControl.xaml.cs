using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls.Property {
    /// <summary>
    /// Interaction logic for DevicePropertyControl.xaml
    /// </summary>
    public partial class DevicePropertyControl : PropertyControl {
        private DeviceProperty _property;

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
        public DevicePropertyControl(ns.Base.Plugins.Properties.Property property, bool isConnectable) 
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            this.NameLabel.Content = property.Name;
            _property = property as DeviceProperty;

            this.SelectionBox.ItemsSource = _property.DevicePlugins;
            this.SelectionBox.DisplayMemberPath = "DisplayName";

            Device device = null;
            if (_property.Value == null) {
                if ((device = _property.DevicePlugins.Find(d => d.Name.Contains("ImageFileDevice")) as Device) != null)
                    this.SelectionBox.SelectedItem = device;
            } else {
                device = _property.Value as Device;
                this.SelectionBox.SelectedItem = device;
            }

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.SelectionBox as Control, this.ConnectImage);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == this.ConnectButton) {
                this.ConnectClicked(this.SelectionBox as Control, this.ConnectImage);
            }
        }

        private void SelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            _property.SetDevice(this.SelectionBox.SelectedItem as Device);
        }
    }
}
