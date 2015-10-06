using ns.Base;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using ns.GUI.WPF.Controls.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : UserControl {

        private const double GRID_HEIGHT = 31.0;
        private Node _selectedNode;

        public PropertyEditor() {
            InitializeComponent();
            this.Loaded += HandleLoaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this) == false) {
                GuiManager guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
                guiManager.SelectedItemChanged += nodeManager_SelectedItemChanged;
            }
        }

        private void nodeManager_SelectedItemChanged(object sender, Base.Event.NodeSelectionChangedEventArgs e) {
            _selectedNode = e.SelectedNode;
            UpdateProperties(_selectedNode);
        }

        private void UpdateProperties(Node selectedNode) {

            this.PropertyGrid.RowDefinitions.Clear();
            this.PropertyGrid.Children.Clear();

            if (selectedNode == null) return;

            RowDefinition baseRowDefinition = new RowDefinition();
            this.PropertyGrid.RowDefinitions.Add(baseRowDefinition);

            GroupBox baseGroupBox = new GroupBox();
            baseGroupBox.Header = "Base";
            baseGroupBox.BorderThickness = new Thickness(0.0);
            this.PropertyGrid.Children.Add(baseGroupBox);

            Grid baseGrid = new Grid();
            baseGroupBox.Content = baseGrid;

            if (selectedNode is StringProperty)
                AddStringProperty(baseGrid, selectedNode, false);
            else
                AddStringProperty(baseGrid, "Name", selectedNode.Name);

            GeneratePropertyControls(baseGrid, selectedNode);
        }

        private bool CheckPropertyIsNumberType(ns.Base.Plugins.Properties.Property property) {
            if(property is NumberProperty<object> || property is DoubleProperty || property is IntegerProperty) {
                return true;
            }

            return false;
        }

        private void GeneratePropertyControls(Grid parentGrid, Node node) {
            foreach (ns.Base.Plugins.Properties.Property property in node.Childs.Where(c => c is ns.Base.Plugins.Properties.Property && !((ns.Base.Plugins.Properties.Property)c).IsOutput)) {
                if (property.IsOutput) continue;

                if (property is StringProperty 
                    || property.GetType().IsAssignableFrom(typeof(StringProperty)) 
                    || property.GetType().IsSubclassOf(typeof(StringProperty))) {
                    AddStringProperty(parentGrid, property, true);
                } else if (CheckPropertyIsNumberType(property)) {
                    AddNumberProperty(parentGrid, property, true);
                } else if (property is DeviceProperty) {
                    AddDeviceProperty(parentGrid, property, true);
                    if (property.Childs.Count > 0) {
                        GroupBox groupBox = new GroupBox();
                        groupBox.BorderThickness = new Thickness(0.0);
                        groupBox.Header = property.GroupName;
                        parentGrid.Children.Add(groupBox);

                        RowDefinition rowDefinition = new RowDefinition();
                        rowDefinition.Height = GridLength.Auto;
                        parentGrid.RowDefinitions.Add(rowDefinition);
                        Grid.SetRow(groupBox, parentGrid.Children.Count);

                        Grid grid = new Grid();
                        groupBox.Content = grid;

                        foreach (Node child in property.Childs) {
                            GeneratePropertyControls(grid, child);
                        }
                    }
                } else if (property is ListProperty) {
                    AddComboBoxProperty(parentGrid, property, true);
                } else if (property is ImageProperty) {
                    AddComboBoxProperty(parentGrid, property, true);
                }else if (property is RectangleProperty) {
                    AddRectangleProperty(parentGrid, property, true);
                }
            }
        }

        private StringPropertyControl AddStringProperty(Grid grid, Node property, bool isConnectable) {
            StringPropertyControl control = new StringPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            control.TextChanged += StringPropertyControlTextChanged;
            SetControlGridPosition(control, grid);
            return control;
        }

        private StringPropertyControl AddStringProperty(Grid grid, string name, string content) {
            StringPropertyControl control = new StringPropertyControl(name, content);
            control.Name = name;
            control.TextChanged += StringPropertyControlTextChanged;
            SetControlGridPosition(control, grid);
            return control;
        }

        private NumberPropertyControl AddNumberProperty(Grid grid, Node property, bool isConnectable) {
            NumberPropertyControl control = new NumberPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            return control;
        }

        private RectanglePropertyControl AddRectangleProperty(Grid grid, Node property, bool isConnectable) {
            RectanglePropertyControl control = new RectanglePropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            return control;
        }

        private DevicePropertyControl AddDeviceProperty(Grid grid, Node property, bool isConnectable) {
            DevicePropertyControl control = new DevicePropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            DeviceProperty deviceProperty = property as DeviceProperty;

            control.SelectionBox.SelectionChanged += SelectionBoxSelectionChanged;
            return control;
        }

        private void SelectionBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateProperties(_selectedNode);
        }

        private ComboBoxPropertyControl AddComboBoxProperty(Grid grid, Node property, bool isConnectable) {
            ComboBoxPropertyControl control = new ComboBoxPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            return control;
        }

        private void SetControlGridPosition(PropertyControl control, Grid grid) {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(GRID_HEIGHT);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
        }

        private void StringPropertyControlTextChanged(object sender, TextChangedEventArgs e) {
            StringPropertyControl spc = sender as StringPropertyControl;
            if (spc.Name == "Name") {
                if (_selectedNode is StringProperty)
                    ((StringProperty)_selectedNode).Value = spc.ContentBox.Text;
                else
                    _selectedNode.Name = spc.ContentBox.Text;
            }
        }
    }
}
