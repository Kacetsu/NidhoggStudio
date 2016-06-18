using ns.Communication.Client;
using ns.Communication.CommunicationModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für AddToolControl.xaml
    /// </summary>
    public partial class AddToolControl : UserControl {
        private Task _task;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToolControl"/> class.
        /// </summary>
        public AddToolControl() {
            InitializeComponent();
            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        private void AddToolToControl(ToolCommunicationModel model) {
            AddToolNodeControl nodeControl = new AddToolNodeControl(model);
            int childCount = ToolGrid.Children.Count;
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            ToolGrid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(nodeControl, childCount);
            ToolGrid.Children.Add(nodeControl);
        }

        private void GeneratePluginList() {
            CategoryComboBox.Dispatcher.Invoke(new Action(() => {
                CategoryComboBox.Items.Clear();
                CategoryComboBox.Items.Add("All");
                CategoryComboBox.SelectedIndex = 0;
            }));

            List<ToolCommunicationModel> models = ClientCommunicationManager.PluginService.GetAvailableTools();

            ToolGrid.Dispatcher.BeginInvoke(new Action(() => {
                foreach (ToolCommunicationModel model in models) {
                    AddToolToControl(model);
                }
            }));

            CategoryComboBox.Dispatcher.BeginInvoke(new Action(() => {
                foreach (ToolCommunicationModel model in models) {
                    if (CategoryComboBox.Items.Contains(model.Category)) continue;

                    CategoryComboBox.Items.Add(model.Category);
                }
            }));
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            _task = new Task(GeneratePluginList);
            _task.Start();
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e) {
            _task?.Wait(TimeSpan.FromSeconds(10));
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            string category = CategoryComboBox.SelectedItem as string;
            foreach (AddToolNodeControl control in ToolGrid.Children) {
                control.Visibility = Visibility.Visible;
            }

            if (category == "All") return;

            foreach (AddToolNodeControl control in ToolGrid.Children) {
                if (!control.Model.Category.Equals(category)) {
                    control.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}