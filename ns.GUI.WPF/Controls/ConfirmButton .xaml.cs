using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für RemoveButton.xaml
    /// </summary>
    public partial class ConfirmButton : UserControl {
        public delegate void ConfirmedHandler(object sender, EventArgs e);
        public event ConfirmedHandler Confirmed;

        private string _text = string.Empty;
        public string Text {
            get { return _text; }
            set { _text = value; }
        }

        public ConfirmButton() {
            InitializeComponent();
            DataContext = this;
            ConfirmGrid.Height = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if(sender == RmButton) {
                GuiHelper.DoubleAnimateControl(0, RmButton, Rectangle.HeightProperty);
                GuiHelper.DoubleAnimateControl(68, ConfirmGrid, Rectangle.HeightProperty);
            }else if(sender == NoButton || sender == YesButton) {
                GuiHelper.DoubleAnimateControl(50, RmButton, Rectangle.HeightProperty);
                GuiHelper.DoubleAnimateControl(0, ConfirmGrid, Rectangle.HeightProperty);
                if(sender == YesButton && Confirmed != null) {
                    Confirmed(this, new EventArgs());
                }
            }
        }
    }
}
