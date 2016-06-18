using ns.Communication.Client;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Nidhogg_Studio {

    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window {
#if DEBUG
        private const long MIN_STARTUP_TIME = 500;
#else
        private const long MIN_STARTUP_TIME = 2000;
#endif
        private Stopwatch _stopwatch = new Stopwatch();

        public SplashWindow() {
            InitializeComponent();
            _stopwatch.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                ClientCommunicationManager.Instance.Initialize();

                _stopwatch.Stop();
                long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds < MIN_STARTUP_TIME) {
                    Thread.Sleep((int)(MIN_STARTUP_TIME - elapsedMilliseconds));
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }));
        }
    }
}