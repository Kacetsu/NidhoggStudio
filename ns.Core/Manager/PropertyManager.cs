using ns.Base;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Base.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace ns.Core.Manager {
    public class PropertyManager : BaseManager {
        private List<Property> _propertyTypes = new List<Property>();
        private List<Property> _properties = new List<Property>();
        private DeviceManager _deviceManager;
        private DisplayManager _displayManager;
        private DataStorageManager _dataStorageManager;

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public List<Property> PropertyTypes {
            get { return _propertyTypes; }
            set { _propertyTypes = value; }
        }

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            AddType(new StringProperty());
            AddType(new IntegerProperty());
            AddType(new DoubleProperty());
            AddType(new DeviceProperty());
            AddType(new ImageProperty());
            AddType(new ListProperty());
            AddType(new OperationSelectionProperty());
            AddType(new Property());
            return true;
        }

        /// <summary>
        /// Adds the specified property.
        /// </summary>
        /// <param name="propertyType">The property.</param>
        /// <returns></returns>
        public bool AddType(Property propertyType) {
            bool result = false;

            if ((result = !_propertyTypes.Contains(propertyType)) == true) {
                _propertyTypes.Add(propertyType);
                Trace.WriteLine("Property type added: " + propertyType.ToString(), LogCategory.Debug);
            }
            
            return result;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(Node node) {
            if (_deviceManager == null)
                _deviceManager = CoreSystem.Managers.Find(m => m.Name.Contains("DeviceManager")) as DeviceManager;
            if (_displayManager == null)
                _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;

            if (!Nodes.Contains(node)) {
                if (node is DeviceProperty) {
                    List<Node> devices = new List<Node>();
                    foreach (Plugin plugin in _deviceManager.Plugins) {
                        if (plugin is Device)
                            devices.Add(plugin as Device);
                    }
                    
                    DeviceProperty deviceProperty = node as DeviceProperty;
                    deviceProperty.AddDeviceList(devices, _deviceManager);
                }

                if (node is Property) {
                    Property property = node as Property;
                    property.PropertyChanged += PropertyPropertyChanged;
                    if (property.IsMonitored) {
                        if (_dataStorageManager == null)
                            _dataStorageManager = CoreSystem.Managers.Find(m => m.Name.Contains("DataStorageManager")) as DataStorageManager;
                        _dataStorageManager.Add(property);
                    }
                }

                _displayManager.Add(node);
                Nodes.Add(node);
                Trace.WriteLine("Property added: " + node.ToString(), LogCategory.Debug);
                OnNodeAdded(node);
            }
        }

        private void PropertyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsMonitored") {
                if(_dataStorageManager == null)
                    _dataStorageManager = CoreSystem.Managers.Find(m => m.Name.Contains("DataStorageManager")) as DataStorageManager;

                Property property = sender as Property;
                if(property.IsMonitored)
                    _dataStorageManager.Add(property);
                else
                    _dataStorageManager.Remove(property);
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Node node) {
            if (_displayManager == null)
                _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;

            if (node is Property) {
                foreach (Property child in node.Childs.Where(p => p is Property))
                    Remove(child);

                _displayManager.Remove(node);
                base.Remove(node);
            }
        }

        /// <summary>
        /// Connects the properties by node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ConnectPropertiesByNode(Node node) {
            foreach (Node c in node.Childs) {
                if (c is Property) {
                    Property child = c as Property;
                    if (!string.IsNullOrEmpty(child.ConnectedToUID)) {
                        Property parent = this.Nodes.Find(p => p.UID == child.ConnectedToUID) as Property;
                        if (parent != null)
                            child.Connect(parent);
                    }
                }

                ConnectPropertiesByNode(c);
            }
        }

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public List<Property> GetConnectableProperties(Property property) {
            Node mainParent = null;
            Node tmpParent = null;
            Tool toolParent = null;
            Tool mainToolParent = null;

            List<Property> result = null;

            while (tmpParent == null || tmpParent.Parent != null) {
                if (tmpParent == null)
                    tmpParent = property.Parent;
                else
                    tmpParent = tmpParent.Parent;

                if (tmpParent is Tool && toolParent == null)
                    toolParent = tmpParent as Tool;
            }

            if (toolParent != null) {
                while (mainToolParent == null || mainToolParent.Parent is Tool) {
                    if (mainToolParent == null)
                        mainToolParent = toolParent;
                    else if (mainToolParent.Parent is Tool) {
                        mainToolParent = mainToolParent.Parent as Tool;
                    }
                }
            }

            mainParent = tmpParent;

            result = GetTargetProperties(property, mainToolParent, mainParent);

            return result;
        }

        /// <summary>
        /// Gets the target properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="targetTool">The target tool.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private List<Property> GetTargetProperties(Property property, Tool targetTool, Node parent) {
            List<ns.Base.Plugins.Properties.Property> properties = new List<ns.Base.Plugins.Properties.Property>();

            foreach (Node node in parent.Childs) {
                if (node == targetTool) break;

                if (node is Property) {
                    ns.Base.Plugins.Properties.Property prop = node as ns.Base.Plugins.Properties.Property;
                    if (prop.IsOutput && (prop.Parent is Tool || prop.Parent is Operation) && property.GetType() == prop.GetType())
                        properties.Add(prop);
                }

                properties.AddRange(GetTargetProperties(property, targetTool, node));
            }

            return properties;
        }

    }
}
