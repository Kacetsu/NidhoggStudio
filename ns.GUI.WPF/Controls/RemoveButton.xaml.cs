using System;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für RemoveButton.xaml
    /// </summary>
    public partial class RemoveButton : UserControl {

        public RemoveButton() {
            InitializeComponent();
            ConfirmGrid.Height = 0d;
        }

        public delegate void EventHandler<EventArgs>(object sender, EventArgs e);

        /// <summary>
        /// Occurs when [remove confirmed].
        /// </summary>
        public event EventHandler<EventArgs> RemoveConfirmed = delegate { };

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == RmButton) {
                GuiHelper.DoubleAnimateControl(0d, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(40d, ConfirmGrid, HeightProperty);
            } else if (sender == NoButton) {
                GuiHelper.DoubleAnimateControl(40d, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(0d, ConfirmGrid, HeightProperty);
            } else if (sender == YesButton) {
                RemoveConfirmed?.Invoke(this, new EventArgs());
            }
        }
    }
}