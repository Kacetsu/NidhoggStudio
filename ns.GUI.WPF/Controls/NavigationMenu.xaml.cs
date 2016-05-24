using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : UserControl, INotifyPropertyChanged {
        private bool _navigationVisible = false;
        private string _pageName = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public string PageName {
            get { return _pageName; }
            set {
                if(_pageName != value) {
                    _pageName = value;
                    OnPropertyChanged("PageName");
                }
            }
        }

        public NavigationMenu() {
            InitializeComponent();
        }

        public void Load(List<NavigationTarget> targets) {

            RowDefinitionCollection rowDefinitions = NavigationTargetsGrid.RowDefinitions;

            int index = 0;
            foreach(NavigationTarget target in targets) { 
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(60);
                rowDefinitions.Add(rowDefinition);

                NavigationTargetControl targetControl = new NavigationTargetControl(target);
                target.Menu = this;
                Grid.SetRow(targetControl, index);
                NavigationTargetsGrid.Children.Add(targetControl);
                index++;
            }
        }

        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private void ControlNavigation() {
            if (!_navigationVisible)
                ShowNavigation();
            else
                HideNavigation();
        }

        private void ShowNavigation() {
            _navigationVisible = true;
            DoubleAnimation animation = new DoubleAnimation(250, TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(Rectangle.WidthProperty, animation);
        }

        private void HideNavigation() {
            _navigationVisible = false;
            DoubleAnimation animation = new DoubleAnimation(60, TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(Rectangle.WidthProperty, animation);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == NavigationButton) {
                ControlNavigation();
            }
        }
    }
}
