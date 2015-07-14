using ns.Base.Log;
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
using ns.Base.Extensions;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für LogViewItem.xaml
    /// </summary>
    public partial class LogViewItem : ListBoxItem {
        public LogViewItem() {
            InitializeComponent();
        }

        public LogViewItem(string timestamp, string message, string category) {
            InitializeComponent();

            this.TimestampLabel.Content = timestamp;
            this.MessageBlock.Text = message;

            SolidColorBrush foregroundBrush;

            if (category == LogCategory.Error.GetDescription())
                foregroundBrush = Application.Current.FindResource("LogErrorForegroundBrush") as SolidColorBrush;
            else if (category == LogCategory.Warning.GetDescription())
                foregroundBrush = Application.Current.FindResource("LogWarningForegroundBrush") as SolidColorBrush;
            else if(category == LogCategory.Debug.GetDescription())
                foregroundBrush = Application.Current.FindResource("LogDebugForegroundBrush") as SolidColorBrush;
            else
                foregroundBrush = Application.Current.FindResource("LogInfoForegroundBrush") as SolidColorBrush;

            this.TimestampLabel.Foreground = foregroundBrush;
            this.MessageBlock.Foreground = foregroundBrush;
        }
    }
}
