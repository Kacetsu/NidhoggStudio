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

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für ResultViewContainer.xaml
    /// </summary>
    public partial class ResultViewContainer : UserControl {
        private ns.Base.Plugins.Properties.Property _property;

        public ns.Base.Plugins.Properties.Property Property {
            get { return _property; }
        }

        public ResultViewContainer(ns.Base.Plugins.Properties.Property property) {
            InitializeComponent();
            DataContext = property;
            _property = property;
            
            if(!(_property is ns.Base.Plugins.Properties.NumberProperty<object>) && !(property is ns.Base.Plugins.Properties.DoubleProperty) && !(property is ns.Base.Plugins.Properties.IntegerProperty)) {
                MinTextBox.IsEnabled = false;
                MaxTextBox.IsEnabled = false;
            }

        }
    }
}
