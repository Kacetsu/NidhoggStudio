using ns.Communication.Client;
using ns.Communication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für AddToolControl.xaml
    /// </summary>
    public partial class AddToolControl : UserControl, INotifyPropertyChanged, IDisposable {
        private bool _isCollapsed = true;
        private Task _task;
        private Thickness _toggleButtonMargin = new Thickness(0, 12, 0, 0);
        private double _toggleButtonRotation = 180d;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToolControl"/> class.
        /// </summary>
        public AddToolControl() {
            InitializeComponent();
            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
            ToggleButton.DataContext = this;
            Height = 60d;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the toggle button margin.
        /// </summary>
        /// <value>
        /// The toggle button margin.
        /// </value>
        public Thickness ToggleButtonMargin {
            get { return _toggleButtonMargin; }
            set {
                _toggleButtonMargin = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the toggle button rotation.
        /// </summary>
        /// <value>
        /// The toggle button rotation.
        /// </value>
        public double ToggleButtonRotation {
            get { return _toggleButtonRotation; }
            set {
                _toggleButtonRotation = value;
                OnPropertyChanged();
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AddToolToControl(ToolModel model) {
            AddToolNodeControl nodeControl = new AddToolNodeControl(model);
            int childCount = ToolGrid.Children.Count;
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            ToolGrid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(nodeControl, childCount);
            ToolGrid.Children.Add(nodeControl);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (_isCollapsed) {
                GuiHelper.DoubleAnimateControl(500d, this, HeightProperty);
                _isCollapsed = false;
                ToggleButtonRotation = 0d;
                ToggleButtonMargin = new Thickness(0);
            } else {
                GuiHelper.DoubleAnimateControl(60d, this, HeightProperty);
                _isCollapsed = true;
                ToggleButtonRotation = 180d;
                ToggleButtonMargin = new Thickness(0, 12, 0, 0);
            }
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

        private void Dispose(bool disposing) {
            if (disposing) {
                _task?.Dispose();
            }
        }

        private void GeneratePluginList() {
            CategoryComboBox.Dispatcher.Invoke(new Action(() => {
                CategoryComboBox.Items.Clear();
                CategoryComboBox.Items.Add("All");
                CategoryComboBox.SelectedIndex = 0;
            }));

            List<ToolModel> models = ClientCommunicationManager.PluginService.GetAvailableTools();

            ToolGrid.Dispatcher.BeginInvoke(new Action(() => {
                foreach (ToolModel model in models) {
                    AddToolToControl(model);
                }
            }));

            CategoryComboBox.Dispatcher.BeginInvoke(new Action(() => {
                foreach (ToolModel model in models) {
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
    }
}