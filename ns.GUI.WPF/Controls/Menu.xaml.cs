using Microsoft.Win32;
using ns.Base;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für Menu.xaml
    /// </summary>
    public partial class Menu : System.Windows.Controls.Menu {
        private FrontendManager _guiManager;

        public Menu() {
            InitializeComponent();
            Loaded += MenuLoaded;
        }

        private void MenuLoaded(object sender, RoutedEventArgs e) {
            try {
                DataContext = _guiManager;

                //foreach (LibraryInformation libraryInformation in pluginManager.LibraryInformations) {
                //    bool contains = false;
                //    foreach (MenuItem item in this.HelpMenuItem.Items) {
                //        if (item is LibraryInformationMenuItem) {
                //            if (((LibraryInformationMenuItem)item).Information == libraryInformation) {
                //                contains = true;
                //                break;
                //            }
                //        }
                //    }
                //    if (contains) continue;

                //    LibraryInformationMenuItem childItem = new LibraryInformationMenuItem(libraryInformation);
                //    childItem.Click += MenuItem_Click;
                //    this.HelpMenuItem.Items.Add(childItem);
                //}
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            if (sender == ExitMenuItem) {
                Application.Current.Shutdown();
            } else if (sender == SaveProjectAsMenuItem) {
                ShowSaveProjectDialog();
            } else if (sender == SaveProjectMenuItem) {
                SaveProject();
            } else if (sender == OpenProjectMenuItem) {
                LoadProject();
            } else if (sender == NewProjectMenuItem) {
                CreateEmptyProject();
            } else if (sender is LibraryInformationMenuItem) {
                LibraryInformationMenuItem item = sender as LibraryInformationMenuItem;
                if (item.Information.DocumentationLink.StartsWith("http")) {
                    Process.Start(item.Information.DocumentationLink);
                }
            }
        }

        private void ShowSaveProjectDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = ProjectManager.FileFilter;
            //if (dialog.ShowDialog() == true) {
            //    ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //    manager.Save(dialog.FileName);
            //}
        }

        private void SaveProject() {
            //ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //if (!string.IsNullOrEmpty(manager?.FileName)) {
            //    ShowSaveProjectDialog();
            //} else {
            //    manager.Save(manager?.FileName);
            //}
        }

        private void LoadProject() {
            MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Loading Project", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                SaveProject();
            } else if (result == MessageBoxResult.Cancel) {
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = ProjectManager.FileFilter;
            //if (dialog.ShowDialog() == true) {
            //    ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //    manager.Load(dialog.FileName);
            //}
        }

        public void CreateEmptyProject() {
            //MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "New Project", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes) {
            //    SaveProject();
            //} else if (result == MessageBoxResult.Cancel) {
            //    return;
            //}

            //ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            //manager.CreateDefaultProject();
        }
    }
}