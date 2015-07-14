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

namespace ns.Plugin.WPF.Statistic {
    /// <summary>
    /// Interaction logic for StatisticTreeItemHeader.xaml
    /// </summary>
    public partial class StatisticTreeItemHeader : UserControl {
        public StatisticTreeItemHeader() {
            InitializeComponent();
        }

        public StatisticTreeItemHeader(string name, string value, Brush color) {
            InitializeComponent();
            this.HeaderText.Text = name;
            this.HeaderValue.Text = value;
            this.ColorBorder.Background = color;
        }

        public void UpdateValue(string value) {
            this.HeaderValue.Text = value;
        }
    }
}
