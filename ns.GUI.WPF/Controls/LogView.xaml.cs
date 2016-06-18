using ns.Base.Log;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
            DebugTestButton.Visibility = Visibility.Visible;
            InfoTestButton.Visibility = Visibility.Visible;
            WarningTestButton.Visibility = Visibility.Visible;
            ErrorTestButton.Visibility = Visibility.Visible;
#endif
            this.Loaded += HandleLoaded;
            LogCollection.CollectionChanged += HandleCollectionChanged;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            //if (CoreSystem.LogListener != null) {
            //    CoreSystem.LogListener.traceListenerEvent -= AddLogEntry;
            //    CoreSystem.LogListener.traceListenerEvent += AddLogEntry;
            //}
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
                    Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                        foreach (LogData logData in e.NewItems) {
                            LogViewItem item = new LogViewItem(logData.Timestamp, logData.Message, logData.Category);
                            if (logList.Items.Count > MAX_BUFFERED_LOGENTRIES) {
                                logList.Items.RemoveAt(0);
                            }
                            logList.Items.Add(item);
                        }
                    }));
                } catch (StackOverflowException ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
#if DEBUG
            if (sender == DebugTestButton)
                Base.Log.Trace.WriteLine("Debug Test", TraceEventType.Verbose);
            else if (sender == InfoTestButton)
                Base.Log.Trace.WriteLine("Info Test", TraceEventType.Information);
            else if (sender == WarningTestButton)
                Base.Log.Trace.WriteLine("Warning Test", TraceEventType.Warning);
            else if (sender == ErrorTestButton)
                Base.Log.Trace.WriteLine("Error Test", TraceEventType.Error);
#endif
        }
    }
}