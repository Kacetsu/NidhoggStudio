using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : UserControl, INotifyPropertyChanged {
        private const double MaxMenuWidth = 45d;
        private bool _navigationVisible = false;
        private string _pageName = string.Empty;

        public NavigationMenu() {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string PageName {
            get { return _pageName; }
            set {
                if (_pageName != value) {
                    _pageName = value;
                    OnPropertyChanged("PageName");
                }
            }
        }

        public void Load(List<NavigationTarget> targets) {
            RowDefinitionCollection rowDefinitions = NavigationTargetsGrid.RowDefinitions;

            int index = 0;
            foreach (NavigationTarget target in targets) {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(MaxMenuWidth);
                rowDefinitions.Add(rowDefinition);

                NavigationTargetControl targetControl = new NavigationTargetControl(target);
                target.Menu = this;
                Grid.SetRow(targetControl, index);
                NavigationTargetsGrid.Children.Add(targetControl);
                index++;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == NavigationButton) {
                ControlNavigation();
            }
        }

        private void ControlNavigation() {
            if (!_navigationVisible)
                ShowNavigation();
            else
                HideNavigation();
        }

        private void HideNavigation() {
            _navigationVisible = false;
            DoubleAnimation animation = new DoubleAnimation(MaxMenuWidth, TimeSpan.FromSeconds(0.2));
            BeginAnimation(WidthProperty, animation);
        }

        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ShowNavigation() {
            _navigationVisible = true;
            DoubleAnimation animation = new DoubleAnimation(250, TimeSpan.FromSeconds(0.2));
            BeginAnimation(WidthProperty, animation);
        }
    }
}