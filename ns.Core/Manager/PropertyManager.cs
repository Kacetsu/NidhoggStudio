using ns.Base;
using ns.Base.Extensions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ns.Core.Manager {

    public class PropertyManager : NodeManager<Property> {
        private DataStorageManager _dataStorageManager;
        private PluginManager _pluginManager;
        private List<Property> _properties = new List<Property>();

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(Property node) {
            if (!Nodes.Contains(node)) {
                if (node is DeviceProperty) {
                    List<Device> devices = new List<Device>();
                    foreach (Device device in _pluginManager.Nodes.Where(n => n is Device)) {
                        devices.Add(device);
                    }

                    DeviceProperty deviceProperty = node as DeviceProperty;
                    deviceProperty.Value = devices.DeepClone();
                }

                if (node is Property) {
                    Property property = node as Property;
                    property.PropertyChanged += PropertyPropertyChanged;
                    if (property.IsMonitored) {
                        if (_dataStorageManager == null)
                            _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();
                        _dataStorageManager.Add(property);
                    }
                }

                Nodes.Add(node);
                Base.Log.Trace.WriteLine("Property added: " + node.ToString(), TraceEventType.Verbose);
                OnNodeAdded(node);
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
                    if (!string.IsNullOrEmpty(child.ConnectedUID)) {
                        Property parent = this.Nodes.Find(p => p.UID == child.ConnectedUID) as Property;
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

        public override bool Initialize() {
            _pluginManager = CoreSystem.FindManager<PluginManager>();
            return _pluginManager != null;
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Property node) {
            foreach (Property child in node.Childs.Where(p => p is Property)) {
                Remove(child);
            }

            base.Remove(node);
        }

        /// <summary>
        /// Gets the target properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="targetTool">The target tool.</param>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        private List<Property> GetTargetProperties(Property property, Tool targetTool, Node parent) {
            List<Property> properties = new List<Property>();

            foreach (Node node in parent.Childs) {
                if (node == targetTool) break;

                if (node is Property) {
                    Property prop = node as Property;
                    if (prop.IsOutput && (prop.Parent is Tool || prop.Parent is Operation) && property.GetType() == prop.GetType())
                        properties.Add(prop);
                }

                properties.AddRange(GetTargetProperties(property, targetTool, node));
            }

            return properties;
        }

        private void PropertyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "IsMonitored") {
                if (_dataStorageManager == null)
                    _dataStorageManager = CoreSystem.FindManager<DataStorageManager>();

                Property property = sender as Property;
                if (property.IsMonitored)
                    _dataStorageManager.Add(property);
                else
                    _dataStorageManager.Remove(property);
            }
        }
    }
}