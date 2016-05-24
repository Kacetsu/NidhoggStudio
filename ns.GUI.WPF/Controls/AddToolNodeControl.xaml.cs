using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
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
    /// Interaktionslogik für AddToolNodeControl.xaml
    /// </summary>
    public partial class AddToolNodeControl : UserControl {
        private Plugin _plugin;

        public AddToolNodeControl() {
            InitializeComponent();
        }

        public AddToolNodeControl(Plugin plugin) {
            InitializeComponent();
            DescriptionTextBlock.Height = 0;
            DescriptionToggleButton.IsChecked = false;
            DataContext = plugin;
            _plugin = plugin;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = double.NaN;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = DescriptionTextBlock.ActualHeight;
            this.DescriptionToggleButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                this.DescriptionTextBlock.Height = 0;
                this.DescriptionTextBlock.ApplyAnimationClock(Rectangle.HeightProperty, null);
                this.DescriptionToggleButton.IsEnabled = true;
            };
            DescriptionTextBlock.BeginAnimation(Rectangle.HeightProperty, animation);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            GuiManager guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;

            if (_plugin is Tool) {
                if(guiManager.SelectedNode == null) {
                    if (projectManager.Configuration.Operations.Count > 0)
                        guiManager.SelectNode(projectManager.Configuration.Operations[0]);
                }else if(guiManager.SelectedNode is Tool) {
                    guiManager.SelectNode(guiManager.SelectedNode.Parent);
                }

                Plugin clone = _plugin.Clone() as Plugin;
                projectManager.Add(clone, guiManager.SelectedNode);

                if (clone is Tool)
                    guiManager.SelectNode(clone);

            } else if(_plugin is Operation) {
                throw new NotSupportedException("Operations are not supperted yet!");
            }
            

        }
    }
}
