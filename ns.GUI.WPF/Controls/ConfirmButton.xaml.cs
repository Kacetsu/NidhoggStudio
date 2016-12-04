using System;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für RemoveButton.xaml
    /// </summary>
    public partial class ConfirmButton : UserControl {
        private string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmButton"/> class.
        /// </summary>
        public ConfirmButton() {
            InitializeComponent();
            DataContext = this;
            ConfirmGrid.Height = 0d;
        }

        public delegate void ConfirmedHandler(object sender, EventArgs e);

        public event ConfirmedHandler Confirmed;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text {
            get { return _text; }
            set { _text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == RmButton) {
                GuiHelper.DoubleAnimateControl(0d, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(40d, ConfirmGrid, HeightProperty);
            } else if (sender == NoButton || sender == YesButton) {
                GuiHelper.DoubleAnimateControl(40d, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(0d, ConfirmGrid, HeightProperty);
                if (sender == YesButton && Confirmed != null) {
                    Confirmed(this, new EventArgs());
                }
            }
        }
    }
}