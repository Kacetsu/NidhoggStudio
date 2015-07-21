using ns.Base;
using ns.Base.Event;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Base.Extensions;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ns.Core.Manager {
    public class ProjectManager : BaseManager {

        private static string _filePath = string.Empty;
        private static string _fileName = string.Empty;
        private const string EXTENSION_ZIP = ".nsproj";
        private const string EXTENSION_XML = ".xml";

        private ProjectConfiguration _configuration = new ProjectConfiguration();
        private PluginManager _pluginManager;
        private PropertyManager _propertyManager;
        private DeviceManager _deviceManager;
        private DisplayManager _displayManager;

        /// <summary>
        /// Will be trickered if a new BaseOperation is added.
        /// </summary>
        /// <param name="sender">The object that is adding a new BaseOperation.</param>
        /// <param name="e">Information about the added Node.</param>
        public delegate void OperationCollectionChangedHandler(object sender, ChildCollectionChangedEventArgs e);

        /// <summary>
        /// Will be triggered if a new BaseOperation is added.
        /// </summary>
        public event OperationCollectionChangedHandler OperationAddedEvent;
        public event OperationCollectionChangedHandler OperationRemovedEvent;

        [XmlIgnore]
        public Action Loaded;

        [XmlIgnore]
        public Action Loading;

        public override bool Initialize() {
            return _configuration.Initialize();
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ProjectConfiguration Configuration {
            get { return _configuration; }
            set { _configuration = value; }
        }

        /// <summary>
        /// Will be invoked when the File Path did changed.
        /// </summary>
        [XmlIgnore]
        public Action FilePathChangedAction;

        /// <summary>
        /// Gets or sets the FilePath.
        /// </summary>
        [XmlIgnore]
        public string FilePath {
            get {
                if (string.IsNullOrEmpty(_filePath) == true)
                    _filePath = ProjectManager.DocumentsPath;
                return _filePath; 
            }
            set {
                _filePath = value;
                FilePathChanged();
            }
        }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        [XmlIgnore]
        public string FileName {
            get {
                if (string.IsNullOrEmpty(_fileName) == true)
                    _fileName = "new" + EXTENSION_ZIP;
                return _fileName;
            }
            set {
                _fileName = value;
                FilePathChanged();
            }
        }

        /// <summary>
        /// Gets the File FullName
        /// </summary>
        public string FileFullName {
            get { return FilePath + FileName; }
        }

        /// <summary>
        /// Gets the File Extension.
        /// </summary>
        public static string FileExtension {
            get { return EXTENSION_ZIP; }
        }

        /// <summary>
        /// Creates the FullName with the path and name.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="name">The File Name.</param>
        public void UpdateFileFullName(string path, string name) {
            _filePath = path;
            _fileName = name;
            FilePathChanged();
        }

        /// <summary>
        /// Files the path changed.
        /// </summary>
        private void FilePathChanged() {
            if (FilePathChangedAction != null)
                FilePathChangedAction();
        }

        /// <summary>
        /// Called when [operation collection changed].
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public void OnOperationCollectionChanged(List<Node> nodes, bool remove) {
            if (!remove) {
                if (this.OperationAddedEvent != null)
                    this.OperationAddedEvent(this, new ChildCollectionChangedEventArgs(nodes));
            } else {
                if (this.OperationRemovedEvent != null)
                    this.OperationRemovedEvent(this, new ChildCollectionChangedEventArgs(nodes));
            }
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Add(Node node) {
            if (_displayManager == null)
                _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;
            if (node is Operation) {
                Operation operation = node as Operation;

                if (!_configuration.Operations.Contains(operation)) {
                    _configuration.Operations.Add(operation);
                    _displayManager.Add(operation);
                    OnOperationCollectionChanged(new List<Node> { node }, false);
                }
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public override void Remove(Node node) {
            if(_displayManager == null)
                _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;

            if (node is Operation) {
                Operation operation = node as Operation;

                if (_configuration.Operations.Contains(operation)) {
                    _configuration.Operations.Remove(operation);
                    OnOperationCollectionChanged(new List<Node> { node }, true);
                }
            } else if (node is Tool) {
                bool enableProcessor = false;

                if (CoreSystem.Processor.IsRunning) {
                    Trace.WriteLine("Stopping processor while removing a tool ...", LogCategory.Info);
                    CoreSystem.Processor.Stop();
                    enableProcessor = true;
                }

                foreach (Node child in node.Childs) {
                    if (child is Property)
                        _propertyManager.Remove(child);
                }

                Node parentNode = node.Parent;
                if (parentNode != null)
                    parentNode.RemoveChild(node);

                if (enableProcessor) {
                    CoreSystem.Processor.Start();
                    Trace.WriteLine("... processor started again!", LogCategory.Info);
                }
            }
            _displayManager.Remove(node);
            base.Remove(node);
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="parent">The parent.</param>
        public void Add(Node node, Node parent) {
            if(_propertyManager == null)
                _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;

            bool enableProcessor = false;

            if (node is Tool && CoreSystem.Processor.IsRunning) {
                Trace.WriteLine("Stopping processor while attaching another tool ...", LogCategory.Info);
                CoreSystem.Processor.Stop();
                enableProcessor = true;
            }

            AddChildNodes(node, parent);

            if (enableProcessor) {
                CoreSystem.Processor.Start();
                Trace.WriteLine("... processor started again!", LogCategory.Info);
            }
        }

        /// <summary>
        /// Adds the child nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="parent">The parent.</param>
        private void AddChildNodes(Node node, Node parent) {
            List<Node> nodes = new List<Node>();
            foreach (Node child in node.Childs) {
                Node childNode = child;
                if (child is Property) {
                    Property originalProperty = child as Property;
                    Property property = originalProperty.DeepClone() as Property;
                    property.UID = Node.GenerateUID();
                    property.SetParent(parent);

                    if (property.CanAutoConnect && !property.IsOutput) {
                        List<Property> connectableProperties = _propertyManager.GetConnectableProperties(property);
                        if (connectableProperties != null && connectableProperties.Count > 0)
                            property.Connect(connectableProperties[connectableProperties.Count - 1]);
                    }
                    childNode = property;
                }
                nodes.Add(childNode);
            }

            node.Childs.Clear();
            parent.AddChild(node);
            node.SetParent(parent);

            foreach (Node child in nodes) {
                node.Childs.Add(child);
                AddChildNodes(child, node);
                _propertyManager.Add(child);
            }
        }

        /// <summary>
        /// Loads a new instance of ProjectManager.
        /// Will also override the old values of the currently loaded ProjectManager.
        /// </summary>
        /// <param name="path">Path to the configuration file.</param>
        /// <returns>Returns the instance of the ProjectManager if successful, else null.</returns>
        public override object Load(string path) {
            if (this.Loading != null)
                this.Loading();

            ProjectManager manager = base.Load(path) as ProjectManager;
            this.Configuration.Name = manager.Configuration.Name;

            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains("PluginManager")) as PluginManager;
            _deviceManager = CoreSystem.Managers.Find(m => m.Name.Contains("DeviceManager")) as DeviceManager;
            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;

            _displayManager.Clear();

            this.Configuration.Operations.Clear();
            _propertyManager.Nodes.Clear();

            List<Node> configuratedDevices = new List<Node>();

            foreach (Device d in manager.Configuration.Devices) {
                Device device = _deviceManager.Plugins.Find(p => p.Fullname == d.Fullname && p.AssemblyFile == d.AssemblyFile) as Device;
                Device deviceClone = device.Clone() as Device;
                deviceClone.Name = d.Name;
                deviceClone.UID = d.UID;
                deviceClone.Cache = d.Cache;

                foreach (Property p in deviceClone.Childs) {
                    if (p is ListProperty) {
                        ListProperty lp = p as ListProperty;
                        Property childProperty = d.Cache.Childs.Find(c => ((Node)c).Name == lp.Name) as Property;
                        lp.Value = childProperty.Value;
                    }
                }

                configuratedDevices.Add(deviceClone);
            }
            List<Device> devices = new List<Device>();
            foreach (Device d in configuratedDevices)
                devices.Add(d);

            this.Configuration.Devices.AddRange(devices);
            _deviceManager.AddRange(configuratedDevices);

            foreach (Operation o in manager.Configuration.Operations) {
                Operation operation = _pluginManager.Plugins.Find(p => p.Fullname == o.Fullname && p.AssemblyFile == o.AssemblyFile) as Operation;
                Operation operationClone = operation.Clone() as Operation;
                operationClone.Name = o.Name;
                operationClone.UID = o.UID;
                operationClone.Cache = o.Cache;

                _displayManager.Add(operationClone);

                CloneProperties(o, operationClone);

                Trace.WriteLine("Loading opeartion: " + operationClone.Name + " (" + operationClone.UID + ")...", LogCategory.Debug);

                LoadToolChilds(o, operationClone);

                this.Configuration.Operations.Add(operationClone);
                _propertyManager.ConnectPropertiesByNode(operationClone);
            }

            if (this.Loaded != null)
                this.Loaded();

            return this;
        }

        /// <summary>
        /// Clones the properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void CloneProperties(Node source, Node destination) {
            List<object> listProperties = destination.Childs.FindAll(c => c is ListProperty);
            List<object> numberProperties = destination.Childs.FindAll(c => c is NumberProperty);

            destination.Childs.Clear();

            foreach (Property p in source.Childs.FindAll(c => c is Property)) {
                Property property = _propertyManager.PropertyTypes.Find(pp => pp.Fullname == p.Fullname) as Property;
                Property propertyClone = property.Clone() as Property;
                propertyClone.Value = p.Value;
                propertyClone.Name = p.Name;
                propertyClone.UID = p.UID;
                propertyClone.IsOutput = p.IsOutput;
                propertyClone.IsMonitored = p.IsMonitored;
                propertyClone.ConnectedToUID = p.ConnectedToUID;
                propertyClone.Childs = new List<object>(p.Childs);

                foreach (Node childNode in propertyClone.Childs)
                    childNode.SetParent(propertyClone);
                propertyClone.SetParent(destination);

                if (propertyClone is DeviceProperty) {
                    string uid = p.Value as string;
                    DeviceProperty devicePropertyClone = propertyClone as DeviceProperty;
                    ns.Base.Plugins.Device device = this.Configuration.Devices.Find(d => d.UID == uid);

                    if (device != null)
                        devicePropertyClone.SetDevice(_deviceManager, device);
                    else
                        devicePropertyClone.SetDevice(_deviceManager, uid);

                } else if (propertyClone is ImageProperty) {
                    _displayManager.Add(propertyClone);
                } else if (propertyClone is ListProperty) {
                    ListProperty originalListProperty = listProperties.Find(o => ((ListProperty)o).Name == propertyClone.Name) as ListProperty;
                    ((ListProperty)propertyClone).List = originalListProperty.List;
                } else if (propertyClone is NumberProperty) {
                    NumberProperty numberPropertyClone = propertyClone as NumberProperty;
                    NumberProperty numberProperty = numberProperties.Find(c => ((NumberProperty)c).Name == numberPropertyClone.Name) as NumberProperty;
                    numberPropertyClone.Min = numberProperty.Min;
                    numberPropertyClone.Max = numberProperty.Max;

                }

                _propertyManager.Add(propertyClone);
                destination.Childs.Add(propertyClone);
            }
        }

        /// <summary>
        /// Loads the tool childs.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="clone">The clone.</param>
        /// <param name="_pluginManager">The manager.</param>
        private void LoadToolChilds(Plugin parent, Plugin clone) {
            foreach (Tool m in parent.Childs.FindAll(c => c is Tool)) {
                Tool tool = _pluginManager.Plugins.Find(p => p.Fullname == m.Fullname && p.AssemblyFile == m.AssemblyFile) as Tool;
                Tool toolClone = tool.Clone() as Tool;
                toolClone.UID = m.UID;
                toolClone.Name = m.Name;

                toolClone.SetParent(clone);
                CloneProperties(m, toolClone);
                
                clone.Childs.Add(toolClone);
                Trace.WriteLine("Added tool: " + toolClone.Name + " (" + toolClone.UID + ")", LogCategory.Debug);
                LoadToolChilds(m, toolClone);
            }
        }
    }
}
