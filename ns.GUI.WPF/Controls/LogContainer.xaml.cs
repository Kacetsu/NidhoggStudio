using ns.Base.Log;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für LogContainer.xaml
    /// </summary>
    public partial class LogContainer : UserControl {
        private string _timestamp;
        private string _message;
        private LogCategory _category;

        public string Timestamp {
            get { return _timestamp; }
        }

        public string Message {
            get { return _message; }
        }

        public LogContainer(string timestamp, string message, LogCategory category) {
            InitializeComponent();
            _timestamp = timestamp;
            _message = message;
            _category = category;
            DataContext = this;
        }
    }
}
