using ns.Base.Manager.ProjectBox;
using ns.Communication.Client;
using ns.GUI.WPF.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF {

    /// <summary>
    /// Interaktionslogik für ProjectView.xaml
    /// </summary>
    public partial class ProjectBoxView : UserControl {
        private ProjectInfoContainer[] _projectContainers;

        public ProjectBoxView() {
            InitializeComponent();
            ProjectControlGrid.Width = 0;
            Loaded += ProjectView_Loaded;
        }

        private void Button_Confirmed(object sender, EventArgs e) {
            if (sender == SaveButton) {
                ClientCommunicationManager.ProjectService.SaveProject();
            }

            //if (sender == SaveButton) {
            //    _projectBoxManager.SaveProject();
            //    UpdateProjectList();
            //} else if (sender == OpenButton) {
            //    _projectBoxManager.LoadProject((ProjectsListBox.SelectedItem as ProjectContainer).InfoContainer.Path);
            //    UpdateProjectList();
            //} else if (sender == NewButton) {
            //    _projectBoxManager.CreateNewProject();
            //    UpdateProjectList();
            //}
        }

        private void ProjectView_Loaded(object sender, RoutedEventArgs e) {
            GuiHelper.DoubleAnimateControl(300, ProjectControlGrid, WidthProperty);
            _projectContainers = ClientCommunicationManager.ProjectService.GetProjects();
            UpdateProjectList();
        }

        private void UpdateProjectList() {
            ProjectsListBox.Items.Clear();
            foreach (ProjectInfoContainer infoContainer in _projectContainers) {
                ProjectsListBox.Items.Add(new ProjectContainer(infoContainer));
            }
        }
    }
}