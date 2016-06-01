using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für LogViewItem.xaml
    /// </summary>
    public partial class LogViewItem : ListBoxItem {

        public LogViewItem() {
            InitializeComponent();
        }

        public LogViewItem(string timestamp, string message, TraceEventType category) {
            InitializeComponent();

            TimestampLabel.Content = timestamp;
            MessageBlock.Text = message;

            SolidColorBrush foregroundBrush;

            if (category == TraceEventType.Error)
                foregroundBrush = Application.Current.FindResource("LogErrorForegroundBrush") as SolidColorBrush;
            else if (category == TraceEventType.Warning)
                foregroundBrush = Application.Current.FindResource("LogWarningForegroundBrush") as SolidColorBrush;
            else if (category == TraceEventType.Verbose)
                foregroundBrush = Application.Current.FindResource("LogDebugForegroundBrush") as SolidColorBrush;
            else
                foregroundBrush = Application.Current.FindResource("LogInfoForegroundBrush") as SolidColorBrush;

            TimestampLabel.Foreground = foregroundBrush;
            MessageBlock.Foreground = foregroundBrush;
        }
    }
}