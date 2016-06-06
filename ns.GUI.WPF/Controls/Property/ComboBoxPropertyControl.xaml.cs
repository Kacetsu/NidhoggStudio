using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for ComboBoxPropertyControl.xaml
    /// </summary>
    public partial class ComboBoxPropertyControl : PropertyControl {
        private Base.Plugins.Properties.Property _property;

        public string DisplayName {
            get { return _property.Name; }
        }

        public ComboBoxPropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        public ComboBoxPropertyControl(Base.Plugins.Properties.Property property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            _property = property;
            DataContext = this;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            } else {
                if (property is ListProperty) {
                    ListProperty listProperty = property as ListProperty;
                    ContentBox.ItemsSource = listProperty.List;

                    if (listProperty.List.Count > 0) {
                        ContentBox.SelectedItem = listProperty.Value;

                        // Value could be a number from any enum,
                        // so we need to check if we found a correct field.
                        if (ContentBox.SelectedItem == null) {
                            if (listProperty.SelectedItem is int)
                                ContentBox.SelectedIndex = (int)listProperty.SelectedItem;
                            else if (listProperty.SelectedItem is string) {
                                foreach (object o in listProperty.List) {
                                    if (o.ToString() == listProperty.Value.ToString()) {
                                        ContentBox.SelectedItem = o;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            }
        }

        private void ContentBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox box = sender as ComboBox;
            ListProperty listProperty = _property as ListProperty;
            if (listProperty != null && listProperty.SelectedItem != box.SelectedItem) {
                listProperty.SelectedItem = box.SelectedItem;
            } else {
                IValue<object> valueProperty = _property as IValue<object>;
                if (valueProperty != null && valueProperty.Value != box.SelectedItem) {
                    valueProperty.Value = box.SelectedItem;
                }
            }
        }
    }
}