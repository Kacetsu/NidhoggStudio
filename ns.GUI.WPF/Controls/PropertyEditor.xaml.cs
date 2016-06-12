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

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public Node Node {
            get { return _node; }
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName {
            get {
                if (_node is Plugin) {
                    Plugin plugin = _node as Plugin;
                    return plugin.Name;
                } else {
                    return "Properties";
                }
            }
            set {
                if (_node is Plugin) {
                    Plugin plugin = _node as Plugin;
                    if (!plugin.Name.Equals(value)) {
                        plugin.Name = value;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
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

            foreach (Node child in _node.Childs) {
                if (child is Base.Plugins.Properties.Property) {
                    Base.Plugins.Properties.Property childProperty = child as ns.Base.Plugins.Properties.Property;
                    if (!childProperty.IsOutput) {
                        UpdateContenGridByProperty(childProperty);
                    }
                }
            }
        }

        private void UpdateContenGridByProperty(Base.Plugins.Properties.Property property) {
            if (property is IntegerProperty) {
                AddIntegerProperty(ContentGrid, property, true);
            } else if (property is DoubleProperty) {
                AddDoubleProperty(ContentGrid, property, true);
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

            foreach (Node childNode in property.Childs) {
                if (childNode is Base.Plugins.Properties.Property) {
                    Base.Plugins.Properties.Property childProperty = childNode as Base.Plugins.Properties.Property;
                    if (!childProperty.IsOutput) {
                        UpdateContenGridByProperty(childProperty);
                    }
                } else if (childNode is Device) {
                    foreach (Base.Plugins.Properties.Property child in childNode.Childs.Where(c => c is Base.Plugins.Properties.Property && !(c as Base.Plugins.Properties.Property).IsOutput)) {
                        UpdateContenGridByProperty(child);
                    }
                }
            }
        }

        private DoublePropertyControl AddDoubleProperty(Grid grid, Node property, bool isConnectable) {
            DoublePropertyControl control = new DoublePropertyControl(property as DoubleProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private IntegerPropertyControl AddIntegerProperty(Grid grid, Node property, bool isConnectable) {
            IntegerPropertyControl control = new IntegerPropertyControl(property as IntegerProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private StringPropertyControl AddStringProperty(Grid grid, Node property, bool isConnectable) {
            StringPropertyControl control = new StringPropertyControl(property as StringProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private ComboBoxPropertyControl AddComboBoxProperty(Grid grid, Node property, bool isConnectable) {
            ComboBoxPropertyControl control = new ComboBoxPropertyControl(property as ListProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private RectanglePropertyControl AddRectangleProperty(Grid grid, Node property, bool isConnectable) {
            RectanglePropertyControl control = new RectanglePropertyControl(property as RectangleProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private DevicePropertyControl AddDeviceProperty(Grid grid, Node property, bool isConnectable) {
            DevicePropertyControl control = new DevicePropertyControl(property as DeviceProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            DeviceProperty deviceProperty = property as DeviceProperty;

            control.SelectionBox.SelectionChanged += SelectionBoxSelectionChanged;
            return control;
        }

        private void SelectionBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateContentGrid();
        }
    }
}