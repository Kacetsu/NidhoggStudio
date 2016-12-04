using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für HistogramControl.xaml
    /// </summary>
    public partial class HistogramControl : UserControl {

        /// <summary>
        /// Initializes a new instance of the <see cref="HistogramControl"/> class.
        /// </summary>
        public HistogramControl() {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the tab button.
        /// </summary>
        /// <value>
        /// The tab button.
        /// </value>
        public Button TabButton => _tabButton;
    }
}