using ns.GUI.WPF.Controls;
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

namespace ns.GUI.WPF {
    /// <summary>
    /// Interaktionslogik für ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl {
        public ProjectView() {
            InitializeComponent();
            ProjectsListBox.Items.Add(new ProjectContainer());
        }

        private void Button_Confirmed(object sender, EventArgs e) {
            if (sender == SaveButton) {

            } else if (sender == OpenButton) {

            } else if (sender == NewButton) {

            } else if (sender == DuplicateButton) {

            }
        }
    }
}
