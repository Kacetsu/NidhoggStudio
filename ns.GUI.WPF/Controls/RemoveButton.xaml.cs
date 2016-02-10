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
    /// Interaktionslogik für RemoveButton.xaml
    /// </summary>
    public partial class RemoveButton : UserControl {
        public RemoveButton() {
            InitializeComponent();
            ConfirmGrid.Height = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if(sender == RmButton) {
                GuiHelper.DoubleAnimateControl(0, RmButton, Rectangle.HeightProperty);
                GuiHelper.DoubleAnimateControl(68, ConfirmGrid, Rectangle.HeightProperty);
            }else if(sender == NoButton) {
                GuiHelper.DoubleAnimateControl(34, RmButton, Rectangle.HeightProperty);
                GuiHelper.DoubleAnimateControl(0, ConfirmGrid, Rectangle.HeightProperty);
            }
        }
    }
}
