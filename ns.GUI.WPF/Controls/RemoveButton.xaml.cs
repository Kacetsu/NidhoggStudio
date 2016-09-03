using System;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für RemoveButton.xaml
    /// </summary>
    public partial class RemoveButton : UserControl {

        /// <summary>
        /// Occurs when [remove confirmed].
        /// </summary>
        public event EventHandler<EventArgs> RemoveConfirmed = delegate { };

        public RemoveButton() {
            InitializeComponent();
            ConfirmGrid.Height = 0;
        }

        public delegate void EventHandler<EventArgs>(object sender, EventArgs e);

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == RmButton) {
                GuiHelper.DoubleAnimateControl(0, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(68, ConfirmGrid, HeightProperty);
            } else if (sender == NoButton) {
                GuiHelper.DoubleAnimateControl(34, RmButton, HeightProperty);
                GuiHelper.DoubleAnimateControl(0, ConfirmGrid, HeightProperty);
            } else if (sender == YesButton) {
                RemoveConfirmed?.Invoke(this, new EventArgs());
            }
        }
    }
}