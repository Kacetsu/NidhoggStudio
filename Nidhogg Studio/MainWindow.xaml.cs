using ns.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using ns.GUI.WPF;

namespace Nidhogg_Studio {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private GuiManager _guiManager;
        private List<NavigationTarget> _mainNavigationTargtes;

        public MainWindow() {
            InitializeComponent();
            Loaded += HandleLoaded;
            Closing += HandleClosing;

            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;

            if (_guiManager == null) {
                _guiManager = new GuiManager();
                _guiManager.Initialize();
                CoreSystem.Managers.Add(_guiManager);
            }
        }

        private void CreateNavigation() {
            Navigation.PropertyChanged += Navigation_PropertyChanged;
            _mainNavigationTargtes = new List<NavigationTarget>();

            BitmapImage editorLogo = new BitmapImage();
            editorLogo.BeginInit();
            editorLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Config.png");
            editorLogo.EndInit();

            BitmapImage monitorLogo = new BitmapImage();
            monitorLogo.BeginInit();
            monitorLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Monitor.png");
            monitorLogo.EndInit();

            BitmapImage statictisLogo = new BitmapImage();
            statictisLogo.BeginInit();
            statictisLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Statistics.png");
            statictisLogo.EndInit();

            BitmapImage logLogo = new BitmapImage();
            logLogo.BeginInit();
            logLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Log.png");
            logLogo.EndInit();

            EditorNavigationTarget projectTarget = new EditorNavigationTarget("Editor", editorLogo);
            MonitorNavigationTarget monitorTarget = new MonitorNavigationTarget("Monitor", monitorLogo);
            StatisticNavigationTarget statisticTarget = new StatisticNavigationTarget("Statistics", statictisLogo);
            LogNavigationTarget logTarget = new LogNavigationTarget("Log", logLogo);
            
            _mainNavigationTargtes.Add(projectTarget);
            _mainNavigationTargtes.Add(monitorTarget);
            _mainNavigationTargtes.Add(statisticTarget);
            _mainNavigationTargtes.Add(logTarget);

            Navigation.Load(_mainNavigationTargtes);

            // Set default page
            Navigation.PageName = "Editor";
        }

        private void Navigation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("PageName")) {
                ContentGird.Children.Clear();
                string pageName = Navigation.PageName;
                switch (pageName) {
                    case "Editor":
                    ContentGird.Children.Add(new Editor());
                    break;
                    case "Monitor":
                    case "Statistic":
                    break;
                    case "Log":
                    ContentGird.Children.Add(new LogView());
                    break;
                    default:
                    throw new NotSupportedException(pageName + " is not supported!");
                }
            }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Title = Title + " (" + version.ToString() + ")";
            CreateNavigation();
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
            if (!CoreSystem.Finalize())
                throw new Exception("Fatal error while closing CoreSystem!");
        }
    }
}
