using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Nidhogg.Service {

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
            CopyrightTextBox.Text = string.Format("{0} {1} {2}", CopyrightTextBox.Text, DateTime.Now.Year, "Levent Tasdemir");
            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                MainWindow mainWindow = new MainWindow();
                _stopwatch.Stop();
                long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

                if (elapsedMilliseconds < MIN_STARTUP_TIME) {
                    Thread.Sleep((int)(MIN_STARTUP_TIME - elapsedMilliseconds));
                }

                Close();
                mainWindow.Show();
            }));
        }
    }
}