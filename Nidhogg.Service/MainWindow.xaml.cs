using ns.Communication;
using ns.Core;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace Nidhogg.Service {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            CommunicationManager.Instance?.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Title = Title + " (" + version.ToString() + ")";
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                CoreSystem.Instance.TryInitialize();
                if (!CoreSystem.Instance.IsInitialized) {
                    throw new TypeInitializationException(nameof(CoreSystem), null);
                }

                CommunicationManager.Instance.Connect();
                ns.Base.Log.Trace.WriteLine(string.Format("Connected establised [{0}]!", CommunicationManager.Instance.IsConnected), CommunicationManager.Instance.IsConnected ? TraceEventType.Information : TraceEventType.Warning);
                DataContext = CommunicationManager.Instance;
            }));
        }
    }
}