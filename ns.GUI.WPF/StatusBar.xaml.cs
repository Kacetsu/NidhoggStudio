using ns.Communication;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ns.GUI.WPF {

    /// <summary>
    /// Interaktionslogik für StatusBar.xaml
    /// </summary>
    public partial class StatusBar : System.Windows.Controls.Primitives.StatusBar, INotifyPropertyChanged {
        private string _connectionType = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusBar"/> class.
        /// </summary>
        public StatusBar() {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the type of the connection.
        /// </summary>
        /// <value>
        /// The type of the connection.
        /// </value>
        public string ConnectionType {
            get { return _connectionType; }
            set {
                _connectionType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void StatusBar_Loaded(object sender, RoutedEventArgs e) {
            ConnectionType = CommunicationManager.Instance.IsConnected ? "Host" : "Client";
        }
    }
}