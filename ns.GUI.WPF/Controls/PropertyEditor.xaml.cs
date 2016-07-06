using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.GUI.WPF.Controls.Property;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Interaktionslogik für PropertyEditor.xaml
    /// </summary>
    public partial class PropertyEditor : UserControl {

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEditor"/> class.
        /// </summary>
        /// <param name="model">The node.</param>
        public PropertyEditor(IPluginModel model) {
            InitializeComponent();
            Model = model;
            DataContext = this;
            UpdateContentGrid();
        }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName {
            get {
                return Model.DisplayName;
            }
            set {
                if (!Model.DisplayName.Equals(value)) {
                    Model.DisplayName = value;
                }
            }
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public IPluginModel Model { get; private set; }

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

        private DoublePropertyControl AddDoubleProperty(Grid grid, Node property, bool isConnectable) {
            DoublePropertyControl control = new DoublePropertyControl(property as DoubleProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private ImagePropertyControl AddImageProperty(Grid grid, Node property, bool isConnectable) {
            ImagePropertyControl control = new ImagePropertyControl(property as ImageProperty, isConnectable);
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

        private ListPropertyControl AddListProperty(Grid grid, Node property, bool isConnectable) {
            ListPropertyControl control = new ListPropertyControl(property as ListProperty, isConnectable);
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

        private StringPropertyControl AddStringProperty(Grid grid, Node property, bool isConnectable) {
            StringPropertyControl control = new StringPropertyControl(property as StringProperty, isConnectable);
            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0, GridUnitType.Auto);
            grid.RowDefinitions.Add(rowDefinition);
            Grid.SetRow(control, grid.Children.Count);
            grid.Children.Add(control);
            return control;
        }

        private bool IsNumberProperty(Base.Plugins.Properties.Property property) => property is INumerical;

        private void SelectionBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            UpdateContentGrid();
        }

        private void UpdateContenGridByProperty(Base.Plugins.Properties.Property property) {
            if (property is IntegerProperty) {
                AddIntegerProperty(ContentGrid, property, true);
            } else if (property is DoubleProperty) {
                AddDoubleProperty(ContentGrid, property, true);
            } else if (property is StringProperty) {
                AddStringProperty(ContentGrid, property, true);
            } else if (property is ImageProperty) {
                AddImageProperty(ContentGrid, property, true);
            } else if (property is RectangleProperty) {
                AddRectangleProperty(ContentGrid, property, true);
            } else if (property is DeviceProperty) {
                AddDeviceProperty(ContentGrid, property, false);
            } else if (property is ListProperty) {
                AddListProperty(ContentGrid, property, true);
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

        private void UpdateContentGrid() {
            ContentGrid.RowDefinitions.Clear();
            ContentGrid.Children.Clear();

            IConfigurableModel configModel = Model as IConfigurableModel;

            foreach (PropertyModel propertyModel in configModel.Properties) {
                if (!propertyModel.Property.IsOutput) {
                    UpdateContenGridByProperty(propertyModel.Property);
                }
            }
        }
    }
}