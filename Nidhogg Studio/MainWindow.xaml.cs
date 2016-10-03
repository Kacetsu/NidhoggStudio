using ns.Communication.Client;
using ns.GUI.WPF;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Nidhogg_Studio {

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //private GuiManager _guiManager;
        private List<NavigationTarget> _mainNavigationTargtes;

        public MainWindow() {
            InitializeComponent();
            Loaded += HandleLoaded;
            Closing += HandleClosing;
        }

        private void CreateNavigation() {
            Navigation.PropertyChanged += Navigation_PropertyChanged;
            _mainNavigationTargtes = new List<NavigationTarget>();

            BitmapImage editorLogo = new BitmapImage();
            editorLogo.BeginInit();
            editorLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Config.png");
            editorLogo.EndInit();

            BitmapImage projectLogo = new BitmapImage();
            projectLogo.BeginInit();
            projectLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Project.png");
            projectLogo.EndInit();

            BitmapImage monitorLogo = new BitmapImage();
            monitorLogo.BeginInit();
            monitorLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Monitor.png");
            monitorLogo.EndInit();

            BitmapImage statisticsLogo = new BitmapImage();
            statisticsLogo.BeginInit();
            statisticsLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Statistics.png");
            statisticsLogo.EndInit();

            BitmapImage logLogo = new BitmapImage();
            logLogo.BeginInit();
            logLogo.UriSource = new Uri("pack://application:,,,/ns.GUI.WPF;component/Images/Log.png");
            logLogo.EndInit();

            EditorNavigationTarget editorTarget = new EditorNavigationTarget("Editor", editorLogo);
            ProjectNavigationTarget projectTarget = new ProjectNavigationTarget("Project", projectLogo);
            MonitorNavigationTarget monitorTarget = new MonitorNavigationTarget("Monitor", monitorLogo);
            monitorTarget.IsEnabled = false;
            StatisticNavigationTarget statisticTarget = new StatisticNavigationTarget("Statistics", statisticsLogo);
            statisticTarget.IsEnabled = false;
            LogNavigationTarget logTarget = new LogNavigationTarget("Log", logLogo);

            _mainNavigationTargtes.Add(editorTarget);
            _mainNavigationTargtes.Add(projectTarget);
            _mainNavigationTargtes.Add(monitorTarget);
            _mainNavigationTargtes.Add(statisticTarget);
            _mainNavigationTargtes.Add(logTarget);

            Navigation.Load(_mainNavigationTargtes);

            // Set default page
            Navigation.PageName = "Editor";
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            FrontendManager.Instance.Close();
            ClientCommunicationManager.Instance.Close();
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            Title = Title + " (" + version.ToString() + ")";
            CreateNavigation();
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

                    case "Project":
                    ContentGird.Children.Add(new ProjectBoxView());
                    break;

                    default:
                    throw new NotSupportedException(pageName + " is not supported!");
                }
            }
        }
    }
}