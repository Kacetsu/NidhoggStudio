using ns.Base.Log;
using ns.Core;
using ns.Core.Manager.ProjectBox;
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
        private ProjectBoxManager _projectBoxManager;

        public ProjectView() {
            InitializeComponent();
            Loaded += ProjectView_Loaded;
        }

        private void UpdateProjectList() {
            if (_projectBoxManager == null) {
                Trace.WriteLine("ProjectView could not find ProjectBoxManager while [Loaded]!", LogCategory.Error);
                return;
            }
            ProjectsListBox.Items.Clear();
            foreach (ProjectInfoContainer infoContainer in _projectBoxManager.ProjectInfos) {
                ProjectsListBox.Items.Add(new ProjectContainer(infoContainer));
            }
        }

        private void UpdateButtonsByProcessorState() {
            if (CoreSystem.Processor.IsRunning) {
                OpenButton.IsEnabled = false;
                NewButton.IsEnabled = false;
            } else {
                OpenButton.IsEnabled = true;
                NewButton.IsEnabled = true;
            }
        }

        private void ProjectView_Loaded(object sender, RoutedEventArgs e) {
            _projectBoxManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectBoxManager")) as ProjectBoxManager;
            UpdateButtonsByProcessorState();
            CoreSystem.Processor.PropertyChanged += Processor_PropertyChanged;
            UpdateProjectList();
        }

        private void Processor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("IsRunning")) {
                UpdateButtonsByProcessorState();
            }
        }

        private void Button_Confirmed(object sender, EventArgs e) {
            if (_projectBoxManager == null)
                _projectBoxManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectBoxManager")) as ProjectBoxManager;

            if (sender == SaveButton) {
                _projectBoxManager.SaveProject();
                UpdateProjectList();
            } else if (sender == OpenButton) {
                _projectBoxManager.LoadProject((ProjectsListBox.SelectedItem as ProjectContainer).InfoContainer.Path);
                UpdateProjectList();
            } else if (sender == NewButton) {
                _projectBoxManager.CreateNewProject();
                UpdateProjectList();
            }
        }
    }
}
