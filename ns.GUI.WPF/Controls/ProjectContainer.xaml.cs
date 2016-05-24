using ns.Core.Manager.ProjectBox;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für ProjectContainer.xaml
    /// </summary>
    public partial class ProjectContainer : UserControl {
        public ProjectInfoContainer InfoContainer {
            get;
            protected set;
        }
        public ProjectContainer(ProjectInfoContainer infoContainer) {
            InitializeComponent();
            GotFocus += ProjectContainer_GotFocus;         
            DataContext = infoContainer;
            InfoContainer = infoContainer;
            if (infoContainer.IsUsed) {
                UsedBorder.Opacity = 1;
            }
            infoContainer.PropertyChanged += InfoContainer_PropertyChanged;
        }

        private void InfoContainer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("IsUsed")) {
                if(InfoContainer.IsUsed)
                    UsedBorder.Opacity = 1;
                else
                    UsedBorder.Opacity = 0.1;
            }
        }

        private void ProjectContainer_GotFocus(object sender, RoutedEventArgs e) {
            if(Parent is ListBox) {
                ListBox parentBox = Parent as ListBox;
                parentBox.SelectedItem = this;
            }
        }
    }
}
