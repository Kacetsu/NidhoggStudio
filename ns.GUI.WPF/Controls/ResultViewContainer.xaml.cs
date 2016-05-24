using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für ResultViewContainer.xaml
    /// </summary>
    public partial class ResultViewContainer : UserControl {
        private Base.Plugins.Properties.Property _property;

        public Base.Plugins.Properties.Property Property {
            get { return _property; }
        }

        public ResultViewContainer(Base.Plugins.Properties.Property property) {
            InitializeComponent();
            DataContext = property;
            _property = property;
            
            if(!(_property is Base.Plugins.Properties.NumberProperty<object>) && !(property is Base.Plugins.Properties.DoubleProperty) && !(property is Base.Plugins.Properties.IntegerProperty)) {
                MinTextBox.IsEnabled = false;
                MaxTextBox.IsEnabled = false;
            }

        }
    }
}
