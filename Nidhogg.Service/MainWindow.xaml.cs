using ns.Communication;
using ns.Core;
using System;
using System.Threading;
using System.Windows;

namespace Nidhogg.Service {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private CommunicationManager _communicationManager;
        private SemaphoreSlim _serviceStopSignal = new SemaphoreSlim(0, 1);

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (!CoreSystem.Initialize()) {
                throw new Exception(string.Format("{0} could not be initialized!", nameof(CoreSystem)));
            }
            _communicationManager = new CommunicationManager();
            _communicationManager.Initialize();
            DataContext = _communicationManager;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _communicationManager.Finalize();
        }
    }
}