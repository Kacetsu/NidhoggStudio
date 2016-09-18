using ns.Communication.Client;
using ns.Communication.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für AddToolNodeControl.xaml
    /// </summary>
    public partial class AddToolNodeControl : UserControl {

        public AddToolNodeControl() {
            InitializeComponent();
        }

        public AddToolNodeControl(ToolModel model) {
            InitializeComponent();
            DescriptionTextBlock.Height = 0;
            DescriptionToggleButton.IsChecked = false;
            DataContext = model;
            Model = model;
        }

        public ToolModel Model { get; private set; }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            //GuiManager guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(GuiManager))) as GuiManager;
            //ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;

            //if (guiManager.SelectedNode == null) {
            //    if (projectManager.Configuration.Operations.Count > 0)
            //        guiManager.SelectNode(projectManager.Configuration.Operations[0]);
            //} else if (guiManager.SelectedNode is Tool) {
            //    guiManager.SelectNode(guiManager.SelectedNode.Parent);
            //}

            //Tool toolCopy = new Tool(tool);
            //projectManager.Add(toolCopy, guiManager.SelectedNode as Operation);
            //guiManager.SelectNode(toolCopy);

            OperationModel operationModel = FrontendManager.SelectedModel as OperationModel;
            if (operationModel != null) {
                ClientCommunicationManager.ProjectService.AddToolToProject(Model, operationModel.Id);
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = double.NaN;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            DescriptionTextBlock.Height = DescriptionTextBlock.ActualHeight;
            DescriptionToggleButton.IsEnabled = false;
            DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.2));
            animation.Completed += delegate (object s, EventArgs ev) {
                DescriptionTextBlock.Height = 0;
                DescriptionTextBlock.ApplyAnimationClock(HeightProperty, null);
                DescriptionToggleButton.IsEnabled = true;
            };
            DescriptionTextBlock.BeginAnimation(HeightProperty, animation);
        }
    }
}