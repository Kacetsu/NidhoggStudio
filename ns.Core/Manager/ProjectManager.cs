﻿using ns.Base;
using ns.Base.Event;
using ns.Base.Extensions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Configuration;
using ns.Core.Manager.ProjectBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ns.Core.Manager {

    public class ProjectManager : BaseManager {
        private static string _fileName = string.Empty;
        private const string EXTENSION_ZIP = ".nsproj";
        private const string EXTENSION_XML = ".xml";
        private const string DEFAULT_PROJECT_FILE = "DefaultProject" + EXTENSION_XML;

        private ProjectConfiguration _configuration = new ProjectConfiguration();
        private PluginManager _pluginManager;
        private PropertyManager _propertyManager;
        private DeviceManager _deviceManager;
        private DisplayManager _displayManager;

        private bool _hasSavedProject = false;

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

        [XmlIgnore]
        public bool HasSavedProject {
            get { return _hasSavedProject; }
            set {
                _hasSavedProject = value;
                OnPropertyChanged("HasSavedProject");
            }
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public ProjectConfiguration Configuration {
            get { return _configuration; }
            set {
                _configuration = value;
                OnPropertyChanged("Configuration");
            }
        }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        [XmlIgnore]
        public string FileName {
            get {
                if (string.IsNullOrEmpty(_fileName) == true)
                    _fileName = DEFAULT_PROJECT_FILE;
                return _fileName;
            }
            set {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// Gets the File Extension.
        /// </summary>
        public static string FileExtension => EXTENSION_ZIP;

        public static string FileFilter => "Project File (*.xml) | *.xml";

        public static string DefaultProjectFile => DEFAULT_PROJECT_FILE;

        /// <summary>
        /// Called when [operation collection changed].
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public void OnOperationCollectionChanged(List<Node> nodes, bool remove) {
            if (!remove) {
                OperationAddedEvent?.Invoke(this, new ChildCollectionChangedEventArgs(nodes));
            } else {
                OperationRemovedEvent?.Invoke(this, new ChildCollectionChangedEventArgs(nodes));
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
            if (_displayManager == null)
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
                    Base.Log.Trace.WriteLine("Stopping processor while removing a tool ...", TraceEventType.Information);
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
                    Base.Log.Trace.WriteLine("... processor started again!", TraceEventType.Information);
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
            if (_propertyManager == null)
                _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;

            bool enableProcessor = false;

            if (node is Tool && CoreSystem.Processor.IsRunning) {
                Base.Log.Trace.WriteLine("Stopping processor while attaching another tool ...", TraceEventType.Information);
                CoreSystem.Processor.Stop();
                enableProcessor = true;
            }

            AddChildNodes(node, parent);

            if (enableProcessor) {
                CoreSystem.Processor.Start();
                Base.Log.Trace.WriteLine("... processor started again!", TraceEventType.Information);
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

        public ProjectManager LoadManager(string path) {
            return base.Load(path) as ProjectManager;
        }

        /// <summary>
        /// Loads a new instance of ProjectManager.
        /// Will also override the old values of the currently loaded ProjectManager.
        /// </summary>
        /// <param name="path">Path to the configuration file.</param>
        /// <returns>Returns the instance of the ProjectManager if successful, else null.</returns>
        public override object Load(string path) {
            if (Loading != null)
                Loading();

            ProjectManager manager = LoadManager(path);
            if (manager == null) return null;
            Configuration.Name = manager.Configuration.Name;

            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains("PluginManager")) as PluginManager;
            _deviceManager = CoreSystem.Managers.Find(m => m.Name.Contains("DeviceManager")) as DeviceManager;
            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;

            _displayManager.Clear();

            Configuration.Initialize();
            Configuration.Operations.Clear();
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

            Configuration.Devices.AddRange(devices);
            _deviceManager.AddRange(configuratedDevices);

            foreach (Operation o in manager.Configuration.Operations) {
                Operation operation = _pluginManager.Plugins.Find(p => p.Fullname == o.Fullname && p.AssemblyFile == o.AssemblyFile) as Operation;
                Operation operationClone = operation.Clone() as Operation;
                operationClone.Name = o.Name;
                operationClone.UID = o.UID;
                operationClone.Cache = o.Cache;

                _displayManager.Add(operationClone);

                CloneProperties(o, operationClone);

                Base.Log.Trace.WriteLine("Loading opeartion: " + operationClone.Name + " (" + operationClone.UID + ")...", TraceEventType.Verbose);

                LoadToolChilds(o, operationClone);

                Configuration.Operations.Add(operationClone);
                _propertyManager.ConnectPropertiesByNode(operationClone);
            }

            Loaded?.Invoke();

            FileName = path;
            HasSavedProject = true;

            return this;
        }

        public override bool Save(string path) {
            if (base.Save(path)) {
                FileName = path;
                HasSavedProject = true;
                return true;
            }

            return false;
        }

        public bool LoadLastUsedProject() {
            ProjectBoxManager projectBoxManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectBoxManager")) as ProjectBoxManager;
            if (projectBoxManager == null) return false;
            if (Load(projectBoxManager.Configuration.LastUsedProjectPath) == null) {
                Base.Log.Trace.WriteLine("ProjectManager could not load last used project!", TraceEventType.Error);
                if (Load(projectBoxManager.DefaultProjectDirectory + ProjectBoxManager.PROJECTFILE_NAME + ProjectBoxManager.EXTENSION_XML) == null) {
                    Base.Log.Trace.WriteLine("ProjectManager could not load default project!", TraceEventType.Error);
                    return false;
                } else {
                    projectBoxManager.Configuration.LastUsedProjectPath = projectBoxManager.DefaultProjectDirectory + ProjectBoxManager.PROJECTFILE_NAME + ProjectBoxManager.EXTENSION_XML;
                    projectBoxManager.SaveProject();
                }
            }

            return true;
        }

        /// <summary>
        /// Clones the properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        private void CloneProperties(Node source, Node destination) {
            List<object> listProperties = destination.Childs.FindAll(c => c is ListProperty);
            ObservableList<object> numberProperties = destination.Childs.DeepClone();

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
                propertyClone.Childs = new ObservableList<object>(p.Childs);
                propertyClone.Tolerance = p.Tolerance;

                foreach (Node childNode in propertyClone.Childs)
                    childNode.SetParent(propertyClone);
                propertyClone.SetParent(destination);

                if (propertyClone is DeviceProperty) {
                    string uid = p.Value as string;
                    DeviceProperty devicePropertyClone = propertyClone as DeviceProperty;
                    Device device = this.Configuration.Devices.Find(d => d.UID == uid);

                    if (device != null)
                        devicePropertyClone.SetDevice(_deviceManager, device);
                    else
                        devicePropertyClone.SetDevice(_deviceManager, uid);
                } else if (propertyClone is ImageProperty) {
                    _displayManager.Add(propertyClone);
                } else if (propertyClone is ListProperty) {
                    ListProperty originalListProperty = listProperties.Find(c => ((Property)c).Name == propertyClone.Name) as ListProperty;
                    ((ListProperty)propertyClone).List = originalListProperty.List;
                } else if (propertyClone is NumberProperty<object>) {
                    NumberProperty<object> targetPropertyClone = propertyClone as NumberProperty<object>;
                    NumberProperty<object> propertyModel = numberProperties.Find(c => ((Property)c).Name == targetPropertyClone.Name) as NumberProperty<object>;
                    if (propertyClone.Tolerance != null) {
                        targetPropertyClone.Tolerance.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Tolerance.Max = propertyModel.Tolerance.Max;
                        targetPropertyClone.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Max = propertyModel.Tolerance.Max;
                    }
                } else if (propertyClone is DoubleProperty) {
                    DoubleProperty targetPropertyClone = propertyClone as DoubleProperty;
                    DoubleProperty propertyModel = numberProperties.Find(c => ((Property)c).Name == targetPropertyClone.Name) as DoubleProperty;
                    if (propertyClone.Tolerance != null) {
                        targetPropertyClone.Tolerance.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Tolerance.Max = propertyModel.Tolerance.Max;
                        targetPropertyClone.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Max = propertyModel.Tolerance.Max;
                    }
                } else if (propertyClone is IntegerProperty) {
                    IntegerProperty targetPropertyClone = propertyClone as IntegerProperty;
                    IntegerProperty propertyModel = numberProperties.Find(c => ((Property)c).Name == targetPropertyClone.Name) as IntegerProperty;
                    if (propertyClone.Tolerance != null) {
                        targetPropertyClone.Tolerance.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Tolerance.Max = propertyModel.Tolerance.Max;
                        targetPropertyClone.Min = propertyModel.Tolerance.Min;
                        targetPropertyClone.Max = propertyModel.Tolerance.Max;
                    }
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
                Base.Log.Trace.WriteLine("Added tool: " + toolClone.Name + " (" + toolClone.UID + ")", TraceEventType.Verbose);
                LoadToolChilds(m, toolClone);
            }
        }
    }
}