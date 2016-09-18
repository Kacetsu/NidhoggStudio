using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.Client;
using ns.Communication.Models.Properties;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for DevicePropertyControl.xaml
    /// </summary>
    public partial class DevicePropertyControl : PropertyControl<DeviceContainerProperty> {
        private bool _isInitializing = true;

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
        public DevicePropertyControl(DeviceContainerProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            IsConnectable = isConnectable;
            DataContext = this;

            SelectionBox.ItemsSource = _property.Items;
            SelectionBox.DisplayMemberPath = "DisplayName";
            ClientCommunicationManager.ProjectService.Callback.PropertyChanged += Callback_PropertyChanged;
            Device device = null;

            if (_property.Value == null) {
                if ((device = _property.Items.Find(d => d.Name.Contains("ImageFileDevice")) as Device) != null) {
                    SelectDevice(device);
                }
            } else {
                device = _property.Value as Device;
                if (device == null) {
                    if ((device = _property.Items.Find(d => d.Name.Contains("ImageFileDevice")) as Device) == null) {
                        device = _property.Items.FirstOrDefault(d => d is Device) as Device;
                    }
                }
                SelectDevice(_property?.Value);
            }

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(SelectionBox as Control, ConnectImage);
            }

            _isInitializing = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(SelectionBox as Control, ConnectImage);
            }
        }

        private void Callback_PropertyChanged(object sender, Communication.Events.PropertyChangedEventArgs e) {
            if (e.Uid.Equals(_property.UID)) {
                SelectionBox.SelectedItem = null;
            }
        }

        private void SelectDevice(Device device) {
            Device deviceSelected = null;
            foreach (Device dev in SelectionBox.Items) {
                if (dev.UID.Equals(device.UID)) {
                    deviceSelected = dev;
                }
            }

            SelectionBox.SelectedItem = deviceSelected;
        }

        private void SelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Device device = SelectionBox.SelectedItem as Device;
            if (device == null || _isInitializing) {
                return;
            }

            e.Handled = true;

            if (_property.Value != device) {
                try {
                    ClientCommunicationManager.ProjectService.ChangePropertyValue(SelectionBox.SelectedItem, _property.UID);
                    _property.Value = device;
                } catch (FaultException ex) {
                    Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Error);
                } finally {
                    SelectionBox.SelectedItem = _property.Value;
                }
            }
        }
    }
}