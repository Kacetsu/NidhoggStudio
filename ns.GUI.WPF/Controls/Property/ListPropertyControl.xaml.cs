using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for ComboBoxPropertyControl.xaml
    /// </summary>
    public partial class ListPropertyControl : PropertyControl<ListProperty> {

        public ListPropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        public ListPropertyControl(ListProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            DataContext = this;

            if (!string.IsNullOrEmpty(Property.ConnectedUID)) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            } else {
                ContentBox.ItemsSource = property.Value;

                if (property.Value.Count > 0) {
                    ContentBox.SelectedItem = property.Value;

                    // Value could be a number from any enum,
                    // so we need to check if we found a correct field.
                    if (ContentBox.SelectedItem == null) {
                        if (property.SelectedItem is int)
                            ContentBox.SelectedIndex = (int)property.SelectedItem;
                        else if (property.SelectedItem is string) {
                            foreach (object o in property.Value) {
                                if (o.ToString() == property.Value.ToString()) {
                                    ContentBox.SelectedItem = o;
                                    break;
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