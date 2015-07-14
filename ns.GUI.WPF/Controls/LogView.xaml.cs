using ns.Base.Log;
using ns.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für LogView.xaml
    /// </summary>
    public partial class LogView : UserControl {

        private const int MAX_BUFFERED_LOGENTRIES = 100;

        private ObservableCollection<LogData> _logCollection = new ObservableCollection<LogData>();

        public ObservableCollection<LogData> LogCollection {
            set { _logCollection = value; }
            get { return _logCollection; }
        }

        public LogView() {
            InitializeComponent();
#if DEBUG
            this.DebugTestButton.Visibility = System.Windows.Visibility.Visible;
            this.InfoTestButton.Visibility = System.Windows.Visibility.Visible;
            this.WarningTestButton.Visibility = System.Windows.Visibility.Visible;
            this.ErrorTestButton.Visibility = System.Windows.Visibility.Visible;
#endif
            this.Loaded += HandleLoaded;
            LogCollection.CollectionChanged += HandleCollectionChanged;
        }

        void HandleLoaded(object sender, RoutedEventArgs e) {
            if (CoreSystem.LogListener != null) {
                CoreSystem.LogListener.traceListenerEvent -= AddLogEntry;
                CoreSystem.LogListener.traceListenerEvent += AddLogEntry;
            }
        }

        
        private void AddLogEntry(object sender, Base.Event.TraceListenerEventArgs e) {
            try {
                LogCollection.Add(new LogData(e.Timestamp, e.Message, e.Category));
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                try {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        foreach (LogData logData in e.NewItems) {
                            LogViewItem item = new LogViewItem(logData.Timestamp, logData.Message, logData.Category);
                            if (this.logList.Items.Count > MAX_BUFFERED_LOGENTRIES) {
                                this.logList.Items.RemoveAt(0);
                            }
                            this.logList.Items.Add(item);
                        }
                    }));
                } catch (StackOverflowException ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
#if DEBUG
            if (sender == this.DebugTestButton)
                Trace.WriteLine("Debug Test", LogCategory.Debug);
            else if(sender == this.InfoTestButton)
                Trace.WriteLine("Info Test", LogCategory.Info);
            else if (sender == this.WarningTestButton)
                Trace.WriteLine("Warning Test", LogCategory.Warning);
            else if (sender == this.ErrorTestButton)
                Trace.WriteLine("Error Test", LogCategory.Error);
#endif
        }
    }
}