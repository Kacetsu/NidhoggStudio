using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : UserControl {
        private bool _navigationVisible = false;

        public NavigationMenu() {
            InitializeComponent();
        }

        public void Load(List<NavigationTarget> targets) {

            RowDefinitionCollection rowDefinitions = NavigationTargetsGrid.RowDefinitions;

            for(int index = 0; index < targets.Count; index++) { 
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(60);
                rowDefinitions.Add(rowDefinition);

                NavigationTargetControl targetControl = new NavigationTargetControl(targets[index]);
                Grid.SetRow(targetControl, index);
                NavigationTargetsGrid.Children.Add(targetControl);
            }
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
