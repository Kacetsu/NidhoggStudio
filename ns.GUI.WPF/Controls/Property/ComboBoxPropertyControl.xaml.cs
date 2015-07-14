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
    /// Interaction logic for ComboBoxPropertyControl.xaml
    /// </summary>
    public partial class ComboBoxPropertyControl : PropertyControl  {
        private ns.Base.Plugins.Properties.Property _property;

        public ComboBoxPropertyControl() {
            InitializeComponent();
        }

        public ComboBoxPropertyControl(ns.Base.Plugins.Properties.Property property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            _property = property;
            this.NameLabel.Content = property.Name;

            if (!string.IsNullOrEmpty(Property.ConnectedToUID)) {
                ConnectClicked(this.ContentBox as Control, this.ConnectImage);
            } else {
                if (property is ListProperty) {
                    ListProperty listProperty = property as ListProperty;
                    this.ContentBox.ItemsSource = listProperty.List;

                    if (listProperty.List.Count > 0) {
                        this.ContentBox.SelectedItem = listProperty.Value;

                        // Value could be a number from any enum, 
                        // so we need to check if we found a correct field.
                        if (this.ContentBox.SelectedItem == null) {
                            if (listProperty.Value is int)
                                this.ContentBox.SelectedIndex = (int)listProperty.Value;
                            else if (listProperty.Value is string) {
                                foreach (object o in listProperty.List) {
                                    if (o.ToString() == listProperty.Value.ToString()) {
                                        this.ContentBox.SelectedItem = o;
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
            if (sender == this.ConnectButton) {
                this.ConnectClicked(this.ContentBox as Control, this.ConnectImage);
            }
        }

        private void ContentBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ComboBox box = sender as ComboBox;
            _property.Value = box.SelectedItem;
        }
    }
}
