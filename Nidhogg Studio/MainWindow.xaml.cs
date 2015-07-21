using ns.Core;
using ns.GUI.WPF;
using System;
using System.Windows;

namespace Nidhogg_Studio {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ns.GUI.WPF.BaseWindow {

       public MainWindow() {
           GuiManager.SetLanguageDictionary();
            InitializeComponent();
            this.Loaded += HandleLoaded;
            this.Closing += HandleClosing;
            if (this.WindowState == WindowState.Maximized)
                this.Maximized = true;
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (CoreSystem.Processor != null && CoreSystem.Processor.IsRunning) {
                MessageBoxResult result = MessageBox.Show("Operation process still running, do you want to terminate the process?", "Process running", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    CoreSystem.Processor.Stop();
                } else {
                    e.Cancel = true;
                    return;
                }
            }
            if (CoreSystem.Finalize() == false)
                throw new Exception("Fatal error while closing CoreSystem!");
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            if (CoreSystem.Initialize(false) == false)
                throw new Exception("Fatal error while loading CoreSystem!");
        }
    }
}
