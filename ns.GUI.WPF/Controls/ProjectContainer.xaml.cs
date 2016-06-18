using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für ProjectContainer.xaml
    /// </summary>
    public partial class ProjectContainer : UserControl {
        //public ProjectInfoContainer InfoContainer {
        //    get;
        //    protected set;
        //}

        public ProjectContainer() {
            InitializeComponent();
        }

        //public ProjectContainer(ProjectInfoContainer infoContainer) {
        //    InitializeComponent();
        //    GotFocus += ProjectContainer_GotFocus;
        //    DataContext = infoContainer;
        //    InfoContainer = infoContainer;
        //    if (infoContainer.IsUsed) {
        //        UsedBorder.Opacity = 1;
        //    }
        //    infoContainer.PropertyChanged += InfoContainer_PropertyChanged;
        //}

        private void InfoContainer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            //if (e.PropertyName.Equals("IsUsed")) {
            //    if (InfoContainer.IsUsed)
            //        UsedBorder.Opacity = 1;
            //    else
            //        UsedBorder.Opacity = 0.1;
            //}
        }

        private void ProjectContainer_GotFocus(object sender, RoutedEventArgs e) {
            if (Parent is ListBox) {
                ListBox parentBox = Parent as ListBox;
                parentBox.SelectedItem = this;
            }
        }
    }
}