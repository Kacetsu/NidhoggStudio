using ns.Base;
using ns.Core;
using ns.Core.Manager;
using ns.GUI.WPF.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ns.GUI.WPF {

    /// <summary>
    /// Interaktionslogik für LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, INotifyPropertyChanged {
        private const uint MAX_BUFFERED_LOGENTRIES = 100;

        private ObservableCollection<LogContainer> _logCollection = new ObservableCollection<LogContainer>();
        private bool _isInfoVisible = true;
        private bool _isWarningVisible = true;
        private bool _isErrorVisible = true;
        private string _xmlContent = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LogContainer> LogCollection {
            set { _logCollection = value; }
            get { return _logCollection; }
        }

        public bool IsInfoVisible {
            get { return _isInfoVisible; }
            set {
                if (_isInfoVisible != value) {
                    _isInfoVisible = value;
                    UpdateLog();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsWarningVisible {
            get { return _isWarningVisible; }
            set {
                if (_isWarningVisible != value) {
                    _isWarningVisible = value;
                    UpdateLog();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsErrorVisible {
            get { return _isErrorVisible; }
            set {
                if (_isErrorVisible != value) {
                    _isErrorVisible = value;
                    UpdateLog();
                    OnPropertyChanged();
                }
            }
        }

        public string XmlContent {
            get { return _xmlContent; }
            set {
                if (!_xmlContent.Equals(value)) {
                    _xmlContent = value;
                    OnPropertyChanged();
                }
            }
        }

        public LogView() {
            InitializeComponent();
            DataContext = this;
            ProjectGrid.Width = 0;
            LogHeaderGrid.Height = 0;
            Loaded += LogView_Loaded;
            LogContainer dummyContainer = new LogContainer("", "", TraceEventType.Information);
            InfoToggleBorder.DataContext = dummyContainer;
            WarningToggleBorder.DataContext = dummyContainer;
            ErrorToggleBorder.DataContext = dummyContainer;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void UpdateLog() {
            if (CoreSystem.LogListener == null) return;
            LogCollection.Clear();
            foreach (Base.Log.LogData logData in CoreSystem.LogListener.LogEntries) {
                AddLogEntry(null, new Base.Event.TraceListenerEventArgs(logData.Timestamp, logData.Message, logData.Category));
            }
        }

        private void LogView_Loaded(object sender, RoutedEventArgs e) {
            GuiHelper.DoubleAnimateControl(500, ProjectGrid, WidthProperty, TimeSpan.FromSeconds(0.3));
            GuiHelper.DoubleAnimateControl(60, LogHeaderGrid, HeightProperty, TimeSpan.FromSeconds(0.3));

            if (CoreSystem.LogListener != null) {
                UpdateLog();
                CoreSystem.LogListener.traceListenerEvent += AddLogEntry;
            }
        }

        private void AddLogEntry(object sender, Base.Event.TraceListenerEventArgs e) {
            try {
                if (!LogListBox.CheckAccess()) {
                    if (LogCollection.Count > MAX_BUFFERED_LOGENTRIES)
                        LogCollection.RemoveAt(0);
                    LogListBox.Dispatcher.Invoke(new Action(() => LogCollection.Add(new LogContainer(e.Timestamp, e.Message, e.Category))));
                } else {
                    if (LogCollection.Count > MAX_BUFFERED_LOGENTRIES)
                        LogCollection.RemoveAt(0);
                    LogCollection.Add(new LogContainer(e.Timestamp, e.Message, e.Category));
                }
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}