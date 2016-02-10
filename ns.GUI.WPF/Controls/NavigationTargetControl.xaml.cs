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
    /// Interaktionslogik für NavigationTargetControl.xaml
    /// </summary>
    public partial class NavigationTargetControl : UserControl {
        private NavigationTarget _target;
        private string _displayName;
        private BitmapImage _icon;

        public string DisplayName {
            get { return _displayName; }
        }

        public BitmapImage Icon {
            get { return _icon; }
        }

        public NavigationTargetControl() {
            InitializeComponent();
        }

        public NavigationTargetControl(NavigationTarget target) {
            InitializeComponent();
            DataContext = this;
            _displayName = target.DisplayName;
            _target = target;
            _icon = target.Icon;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            _target.CallAction();
        }
    }
}
