using Microsoft.Win32;
using ns.Base;
using ns.Base.Log;
using ns.Core;
using ns.Core.Manager;
using ns.GUI.WPF.Windows;
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
    /// Interaktionslogik für Menu.xaml
    /// </summary>
    public partial class Menu : System.Windows.Controls.Menu {
        public Menu() {
            InitializeComponent();
            this.Loaded += MenuLoaded;
        }

        private void MenuLoaded(object sender, RoutedEventArgs e) {
            try {
                PluginManager pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains("PluginManager")) as PluginManager;

                foreach (LibraryInformation libraryInformation in pluginManager.LibraryInformations) {
                    bool contains = false;
                    foreach (MenuItem item in this.HelpMenuItem.Items) {
                        if (item is LibraryInformationMenuItem) {
                            if (((LibraryInformationMenuItem)item).Information == libraryInformation) {
                                contains = true;
                                break;
                            }
                        }
                    }
                    if (contains) continue;

                    LibraryInformationMenuItem childItem = new LibraryInformationMenuItem(libraryInformation);
                    childItem.Click += MenuItem_Click;
                    this.HelpMenuItem.Items.Add(childItem);
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            if(sender == this.ExitMenuItem)
                Application.Current.Shutdown();
            else if (sender == this.AddElementMenuItem) {
                AddNewElementDialog dialog = new AddNewElementDialog();
                dialog.ShowDialog();
            } else if (sender == this.SaveProjectAsMenuItem) {
                ShowSaveDialog();
            } else if (sender == this.OpenProjectMenuItem) {
                Load();
            } else if (sender is LibraryInformationMenuItem) {
                LibraryInformationMenuItem item = sender as LibraryInformationMenuItem;
                if (item.Information.DocumentationLink.StartsWith("http")) {
                    System.Diagnostics.Process.Start(item.Information.DocumentationLink);
                }
            }
        }

        private void ShowSaveDialog() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Project File (*.xml)|*.xml";
            if (dialog.ShowDialog() == true) {
                ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
                manager.Save(dialog.FileName);
                this.SaveProjectMenuItem.IsEnabled = true;
            }
        }

        private void Load(){
            MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Loading Project", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                //Save();
            } else if (result == MessageBoxResult.Cancel) {
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Project File (*.xml)|*.xml";
            if (dialog.ShowDialog() == true) {
                ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
                manager.Load(dialog.FileName);
            }
        }
    }
}
