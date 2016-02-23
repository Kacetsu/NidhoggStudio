using ns.Base.Log;
using ns.Core;
using ns.GUI.WPF.Controls;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using ns.Core.Manager;
using System.IO;
using System.ComponentModel;

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
                if(_isInfoVisible != value) {
                    _isInfoVisible = value;
                    UpdateLog();
                    OnPropertyChanged("IsInfoVisible");
                }
            }
        }

        public bool IsWarningVisible {
            get { return _isWarningVisible; }
            set {
                if (_isWarningVisible != value) {
                    _isWarningVisible = value;
                    UpdateLog();
                    OnPropertyChanged("IsWarningVisible");
                }
            }
        }

        public bool IsErrorVisible {
            get { return _isErrorVisible; }
            set {
                if (_isErrorVisible != value) {
                    _isErrorVisible = value;
                    UpdateLog();
                    OnPropertyChanged("IsErrorVisible");
                }
            }
        }

        public string XmlContent {
            get { return _xmlContent; }
            set {
                if (!_xmlContent.Equals(value)) {
                    _xmlContent = value;
                    OnPropertyChanged("XmlContent");
                }
                
            }
        }

        public LogView() {
            InitializeComponent();
            DataContext = this;
            ProjectGrid.Width = 0;
            LogHeaderGrid.Height = 0;
            Loaded += LogView_Loaded;
            LogContainer dummyContainer = new LogContainer("", "", LogCategory.Info);
            InfoToggleBorder.DataContext = dummyContainer;
            WarningToggleBorder.DataContext = dummyContainer;
            ErrorToggleBorder.DataContext = dummyContainer;
        }

        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void UpdateLog() {
            if (CoreSystem.LogListener == null) return;
            LogCollection.Clear();
            foreach (LogData logData in CoreSystem.LogListener.LogEntries) {
                AddLogEntry(null, new Base.Event.TraceListenerEventArgs(logData.Timestamp, logData.Message, logData.Category));
            }
        }

        private void LogView_Loaded(object sender, RoutedEventArgs e) {
            GuiHelper.DoubleAnimateControl(500, ProjectGrid, Rectangle.WidthProperty, TimeSpan.FromSeconds(0.3));
            GuiHelper.DoubleAnimateControl(60, LogHeaderGrid, Rectangle.HeightProperty, TimeSpan.FromSeconds(0.3));

            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            if (projectManager == null) {
                Trace.WriteLine("Could not find project manager!", LogCategory.Error);
            } else {
                MemoryStream stream = new MemoryStream();
                projectManager.Save(ref stream);
                XmlContent = Encoding.UTF8.GetString(stream.ToArray());
            }

            if (CoreSystem.LogListener != null) {
                UpdateLog();
                CoreSystem.LogListener.traceListenerEvent += AddLogEntry;
            }
        }

        private void AddLogEntry(object sender, Base.Event.TraceListenerEventArgs e) {
            try {
                LogCategory category = LogCategory.Error;
                switch (e.Category) {
                    case "ERROR":
                    category = LogCategory.Error;
                    if (!IsErrorVisible) return;
                    break;
                    case "WARNING":
                    category = LogCategory.Warning;
                    if (!IsWarningVisible) return;
                    break;
                    case "INFO":
                    category = LogCategory.Info;
                    if (!IsInfoVisible) return;
                    break;
                    case "DEBUG":
                    category = LogCategory.Debug;
                    break;
                    default:
                    throw new NotSupportedException(e.Category + " is not supported!");
                }
                if (!LogListBox.CheckAccess()) {
                    if (LogCollection.Count > MAX_BUFFERED_LOGENTRIES)
                        LogCollection.RemoveAt(0);
                    LogListBox.Dispatcher.Invoke(new Action(() => LogCollection.Add(new LogContainer(e.Timestamp, e.Message, category))));
                } else {
                    if (LogCollection.Count > MAX_BUFFERED_LOGENTRIES)
                        LogCollection.RemoveAt(0);
                    LogCollection.Add(new LogContainer(e.Timestamp, e.Message, category));
                }
            } catch (StackOverflowException ex) {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
