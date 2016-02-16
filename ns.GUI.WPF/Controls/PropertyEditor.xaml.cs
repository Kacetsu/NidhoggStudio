using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.GUI.WPF.Controls.Property;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaktionslogik für PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : UserControl {
        private Node _node;

        public Node Node {
            get { return _node; }
        }

        public string DisplayName {
            get {
                if(_node is Plugin) {
                    Plugin plugin = _node as Plugin;
                    return plugin.DisplayName;
                } else {
                    return "Properties";
                }
            }
        }

        public PropertyEditor(Node node) {
            InitializeComponent();
            _node = node;
            DataContext = this;
            UpdateContentGrid();
        }

        private bool IsNumberProperty(ns.Base.Plugins.Properties.Property property) {
            if (property is NumberProperty<object> || property is DoubleProperty || property is IntegerProperty) {
                return true;
            }

            return false;
        }

        private void UpdateContentGrid() {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();

            foreach(Node child in _node.Childs) {
                if(child is  ns.Base.Plugins.Properties.Property) {
                    ns.Base.Plugins.Properties.Property childProperty = child as ns.Base.Plugins.Properties.Property;
                    if (!childProperty.IsOutput) {
                        UpdateContenGridByProperty(childProperty);
                    }
                }
            }
        }

        private void UpdateContenGridByProperty(Base.Plugins.Properties.Property property) {
            if (IsNumberProperty(property)) {
                // Is it a NumberProperty we only have to create one type of control.
                AddNumberProperty(ContentGrid, property, true);
            } else if (property is StringProperty) {
                AddStringProperty(ContentGrid, property, true);
            } else if (property is ImageProperty) {
                AddComboBoxProperty(ContentGrid, property, true);
            } else if (property is RectangleProperty) {
                AddRectangleProperty(ContentGrid, property, true);
            } else if (property is DeviceProperty) {
                AddDeviceProperty(ContentGrid, property, false);
            } else if (property is ListProperty) {
                AddComboBoxProperty(ContentGrid, property, true);
            }

            foreach(Node childNode in property.Childs) {
                if (childNode is Base.Plugins.Properties.Property) {
                    Base.Plugins.Properties.Property childProperty = childNode as Base.Plugins.Properties.Property;
                    if (!childProperty.IsOutput) {
                        UpdateContenGridByProperty(childProperty);
                    }
                } else if(childNode is Device) {
                    foreach(Base.Plugins.Properties.Property child in childNode.Childs.Where(c => c is Base.Plugins.Properties.Property && !(c as Base.Plugins.Properties.Property).IsOutput)) {
                        UpdateContenGridByProperty(child);
                    }
                }
            }
        }

        private void SetControlGridPosition(PropertyControl control, Grid grid) {
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
        }

        private NumberPropertyControl AddNumberProperty(Grid grid, Node property, bool isConnectable) {
            NumberPropertyControl control = new NumberPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            return control;
        }

        private StringPropertyControl AddStringProperty(Grid grid, Node property, bool isConnectable) {
            StringPropertyControl control = new StringPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
            SetControlGridPosition(control, grid);
            return control;
        }

        private ComboBoxPropertyControl AddComboBoxProperty(Grid grid, Node property, bool isConnectable) {
            ComboBoxPropertyControl control = new ComboBoxPropertyControl(property as ns.Base.Plugins.Properties.Property, isConnectable);
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
            UpdateContentGrid();
        }
    }
}
