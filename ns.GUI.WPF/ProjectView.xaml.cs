using ns.GUI.WPF.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF {

    /// <summary>
    /// Interaktionslogik für ProjectView.xaml
    /// </summary>
    public partial class ProjectView : UserControl {

        public ProjectView() {
            InitializeComponent();
            Loaded += ProjectView_Loaded;
        }

        private void UpdateProjectList() {
            //if (_projectBoxManager == null) {
            //    Base.Log.Trace.WriteLine("ProjectView could not find ProjectBoxManager while [Loaded]!", TraceEventType.Error);
            //    return;
            //}
            ProjectsListBox.Items.Clear();
            //foreach (ProjectInfoContainer infoContainer in _projectBoxManager.ProjectInfos) {
            //    ProjectsListBox.Items.Add(new ProjectContainer(infoContainer));
            //}
        }

        private void UpdateButtonsByProcessorState() {
            //if (CoreSystem.Processor.IsRunning) {
            //    OpenButton.IsEnabled = false;
            //    NewButton.IsEnabled = false;
            //} else {
            //    OpenButton.IsEnabled = true;
            //    NewButton.IsEnabled = true;
            //}
        }

        private void ProjectView_Loaded(object sender, RoutedEventArgs e) {
            //_projectBoxManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectBoxManager")) as ProjectBoxManager;
            //UpdateButtonsByProcessorState();
            //CoreSystem.Processor.PropertyChanged += Processor_PropertyChanged;
            //UpdateProjectList();
        }

        private void Processor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("IsRunning")) {
                UpdateButtonsByProcessorState();
            }
        }

        private void Button_Confirmed(object sender, EventArgs e) {
            //if (_projectBoxManager == null)
            //    _projectBoxManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectBoxManager")) as ProjectBoxManager;

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
    }
}