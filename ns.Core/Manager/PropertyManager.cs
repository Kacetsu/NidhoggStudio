using ns.Base;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ns.Core.Manager {

    public class PropertyManager : NodeManager<Property> {
        private PluginManager _pluginManager;
        private List<Property> _properties = new List<Property>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyManager"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PropertyManager([CallerMemberName] string name = null) : base(name) {
            _pluginManager = CoreSystem.Instance.Plugins;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="property">The node.</param>
        public override void Add(Property property) {
            if (!Items.ContainsKey(property.Id)) {
                Items.TryAdd(property.Id, property);
                Base.Log.Trace.WriteLine("Property added: " + property.ToString(), TraceEventType.Verbose);
                OnNodeAdded(property);
            }
        }

        /// <summary>
        /// Connects the properties by node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ConnectPropertiesByNode(Node node) {
            foreach (Node c in node.Items.Values) {
                if (c is Property) {
                    Property child = c as Property;
                    if (!Guid.Empty.Equals(child.ConnectedId)) {
                        Property parent = Items.Values.OfType<Property>().First(p => p.Id == child.ConnectedId);
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
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Property node) {
            foreach (Property child in node.Items.Values.OfType<Property>()) {
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

            foreach (Node node in parent.Items.Values) {
                if (node == targetTool) break;

                if (node is Property) {
                    Property prop = node as Property;
                    if (prop.Direction == PropertyDirection.Out && (prop.Parent is Tool || prop.Parent is Operation) && property.GetType() == prop.GetType())
                        properties.Add(prop);
                }

                properties.AddRange(GetTargetProperties(property, targetTool, node));
            }

            return properties;
        }
    }
}