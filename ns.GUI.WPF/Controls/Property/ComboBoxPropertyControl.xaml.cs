using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {
    /// <summary>
    /// Interaction logic for ComboBoxPropertyControl.xaml
    /// </summary>
    public partial class ComboBoxPropertyControl : PropertyControl  {
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

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
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
                            if (listProperty.Value is int)
                                ContentBox.SelectedIndex = (int)listProperty.Value;
                            else if (listProperty.Value is string) {
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
            if(_property.Value != box.SelectedItem)
                _property.Value = box.SelectedItem;
        }
    }
}
