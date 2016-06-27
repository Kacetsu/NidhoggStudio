using ns.Base.Plugins.Properties;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls.Property {

    /// <summary>
    /// Interaction logic for ComboBoxPropertyControl.xaml
    /// </summary>
    public partial class ImagePropertyControl : PropertyControl<ImageProperty> {

        public ImagePropertyControl() {
            InitializeComponent();
            DataContext = this;
        }

        public ImagePropertyControl(ImageProperty property, bool isConnectable)
            : base(property) {
            InitializeComponent();
            DataContext = this;
            ConnectClicked(ContentBox as Control, ConnectImage);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == ConnectButton) {
                ConnectClicked(ContentBox as Control, ConnectImage);
            }
        }

        private void ContentBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //ComboBox box = sender as ComboBox;
            //ListProperty listProperty = _property as ListProperty;
            //if (listProperty != null && listProperty.SelectedItem != box.SelectedItem) {
            //    listProperty.SelectedItem = box.SelectedItem;
            //} else {
            //    IValue<object> valueProperty = _property as IValue<object>;
            //    if (valueProperty != null && valueProperty.Value != box.SelectedItem) {
            //        valueProperty.Value = box.SelectedItem;
            //    }
            //}
        }
    }
}