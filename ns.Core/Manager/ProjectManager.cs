using ns.Base;
using ns.Base.Exceptions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Devices;
using ns.Base.Plugins.Properties;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Core.Manager {

    public class ProjectManager : GenericConfigurationManager<ProjectConfiguration>, IProjectManager {

        /// <summary>
        /// The default project file
        /// </summary>
        public const string DefaultProjectFile = "DefaultProject" + FileExtension;

        /// <summary>
        /// The file extension
        /// </summary>
        public const string FileExtension = ".xml";

        /// <summary>
        /// Gets the file filter.
        /// </summary>
        /// <value>
        /// The file filter.
        /// </value>
        public const string FileFilter = "Project File (*.xml) | *.xml";

        private static string _fileName = string.Empty;

        private PluginManager _pluginManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager"/> class.
        /// </summary>
        /// <param name="name"></param>
        public ProjectManager([CallerMemberName] string name = null) : base(name) {
            Configuration = new ProjectConfiguration();
        }

        /// <summary>
        /// Gets or sets the FileName.
        /// </summary>
        public string FileName {
            get {
                if (string.IsNullOrEmpty(_fileName) == true) _fileName = DefaultProjectFile;
                return _fileName;
            }
            set {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Adds the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationAlreadyExistsException"></exception>
        public void Add(Operation operation) {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            if (Configuration.Operations.Contains(operation)) throw new OperationAlreadyExistsException(operation.Name);

            PluginManager pluginManager = CoreSystem.Instance.Plugins;

            if (pluginManager == null) {
                throw new ManagerInitializeException(nameof(pluginManager));
            }

            operation.KnownDevices = new Base.Collections.ObservableConcurrentDictionary<Guid, string>();

            foreach (Device device in CoreSystem.Instance.Devices.Items.Values.OfType<Device>()) {
                operation.KnownDevices.TryAdd(device.Id, device.Name);
            }

            if (operation.SelectedDevice != Guid.Empty) {
                ImageDevice device = Configuration.Devices.OfType<ImageDevice>().FirstOrDefault(d => d.Id.Equals(operation.SelectedDevice));
                if (device != null) {
                    operation.CaptureDevice = device;
                }
            }

            if (CoreSystem.Instance.Processor?.State == ProcessorState.Running) {
                operation.Initialize();
            }

            Configuration.Operations.Add(operation);

            foreach (Property property in operation.Items.Values.OfType<Property>().Where(p => p.CanAutoConnect)) {
                List<Property> connectableProperties = FindConnectableProperties(property);
                if (connectableProperties.Count > 0) {
                    property.Connect(connectableProperties[0]);
                }
            }
        }

        /// <summary>
        /// Adds the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OperationNotFoundException"></exception>
        /// <exception cref="ToolAlreadyExistsException"></exception>
        public void Add(Tool tool, Operation parent) {
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            if (parent == null) throw new ArgumentNullException(nameof(parent));

            if (!Configuration.Operations.Contains(parent)) throw new OperationNotFoundException(parent.Name);

            if (parent.Items.Values.Contains(tool)) throw new ToolAlreadyExistsException(tool.Name);

            if (CoreSystem.Instance.Processor?.State == ProcessorState.Running) {
                tool.Initialize();
            }

            parent.AddChild(tool);

            foreach (Property property in tool.Items.Values.OfType<Property>().Where(p => p.CanAutoConnect)) {
                List<Property> connectableProperties = FindConnectableProperties(property);
                if (connectableProperties.Count > 0) {
                    property.Connect(connectableProperties[0]);
                }
            }
        }

        /// <summary>
        /// Adds the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="parent">The parent.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        public void Add(Device device, Operation parent) {
            if (device == null) throw new ArgumentNullException(nameof(device));

            if (parent == null) throw new ArgumentNullException(nameof(parent));

            DeviceManager deviceManager = CoreSystem.Instance.Devices;

            if (deviceManager == null) throw new ManagerInitializeException(nameof(deviceManager));

            deviceManager.Add(device);

            parent.CaptureDevice = device as ImageDevice;
        }

        /// <summary>
        /// Clears the images.
        /// </summary>
        /// <param name="node">The node.</param>
        public void ClearImages(Node node) {
            foreach (Node item in node.Items.Values) {
                ImageProperty imageProperty = item as ImageProperty;
                if (imageProperty != null && imageProperty.Direction == PropertyDirection.Out) {
                    imageProperty.Value = new ImageContainer();
                }

                ClearImages(item);
            }
        }

        /// <summary>
        /// Clears the images.
        /// </summary>
        public void ClearImages() {
            foreach (Operation operation in Configuration.Operations) {
                ClearImages(operation);
            }
        }

        /// <summary>
        /// Creates the default project.
        /// </summary>
        public void CreateDefaultProject() {
            Configuration.Operations.Clear();
            Configuration.Name.Value = "Default";

            PluginManager pluginManager = CoreSystem.Instance.Plugins;

            if (pluginManager == null) {
                throw new ManagerInitializeException(nameof(pluginManager));
            }

            Operation operation = new Operation("Unknown Operation");
            Device device = pluginManager.Items.Values.First(n => n.Name.Equals("ns.Plugin.Base.ImageFileDevice")) as Device;

            if (device != null) {
                Add(device, operation);
            }

            Add(operation);
        }

        /// <summary>
        /// Finds the connectable properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public List<Property> FindConnectableProperties(Property property) {
            List<Property> result = new List<Property>();
            if (property.Direction == PropertyDirection.Out) return result;

            Operation operation = FindOperation(property);
            Tool tool = FindTool(property);

            foreach (Node childNode in operation.Items.Values) {
                Tool childTool = childNode as Tool;
                Property childProperty = childNode as Property;

                if (childTool != null) {
                    if (childTool.Equals(tool)) break;

                    foreach (Property p in childTool.Items.Values.OfType<Property>()) {
                        if (p.GetType().Equals(property.GetType()) && p.Direction == PropertyDirection.Out) {
                            result.Add(p);
                        }
                    }
                } else if (childProperty != null && childProperty.GetType().Equals(property.GetType()) && childProperty.Direction == PropertyDirection.Out) {
                    result.Add(childProperty);
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the operation.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public Operation FindOperation(Property property) {
            foreach (Operation operation in Configuration.Operations) {
                foreach (Node node in operation.Items.Values) {
                    Tool childTool = node as Tool;
                    Property childProperty = node as Property;

                    if (childTool != null) {
                        foreach (Property p in childTool.Items.Values.OfType<Property>()) {
                            if (p == property) {
                                return operation;
                            }
                        }
                    } else if (childProperty == property) {
                        return operation;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the property.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Property FindProperty(Guid id) {
            Property property = null;
            foreach (Operation operation in Configuration.Operations) {
                if (operation.Items.ContainsKey(id)) {
                    property = operation.Items[id] as Property;
                    break;
                }

                ImageDevice tmpDevice = operation.CaptureDevice;
                if (tmpDevice?.Items.ContainsKey(id) == true) {
                    property = tmpDevice.Items[id] as Property;
                    break;
                }

                foreach (Tool tool in operation.Items.Values.OfType<Tool>()) {
                    if (tool.Items.ContainsKey(id)) {
                        property = tool.Items[id] as Property;
                        break;
                    }
                }
            }

            return property;
        }

        /// <summary>
        /// Finds the tool.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <exception cref="PluginNotFoundException"></exception>
        public Tool FindTool(Property property) {
            Operation operation = FindOperation(property);
            foreach (Tool tool in operation.Items.Values.OfType<Tool>()) {
                if (tool.Items.Values.Contains(property)) return tool;
            }

            return null;
        }

        /// <summary>
        /// Finds the tool.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Tool FindTool(Guid id) {
            foreach (Operation operation in Configuration.Operations) {
                Tool tool = operation.Items[id] as Tool;
                return tool;
            }

            return null;
        }

        /// <summary>
        /// Initializes the operations.
        /// </summary>
        /// <exception cref="OperationInitializeException"></exception>
        public void InitializeOperations() {
            PluginManager pluginManager = CoreSystem.Instance.Plugins;
            foreach (Operation operation in Configuration.Operations) {
                operation.KnownDevices = new Base.Collections.ObservableConcurrentDictionary<Guid, string>();

                foreach (Device device in CoreSystem.Instance.Devices.Items.Values.OfType<Device>()) {
                    operation.KnownDevices.TryAdd(device.Id, device.Name);
                }

                operation.Initialize();
            }
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>
        /// The manager object. NULL if any error happend.
        /// </returns>
        /// <exception cref="ManagerInitializeException"></exception>
        /// <exception cref="PluginNotFoundException"></exception>
        /// <exception cref="DeviceInitializeException"></exception>
        public override ProjectConfiguration Load(Stream stream) {
            if (_pluginManager == null) {
                _pluginManager = CoreSystem.Instance.Plugins;
            }

            try {
                stream.Position = 0;
                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), _pluginManager?.KnownTypes);
                Configuration = serializer.ReadObject(stream) as ProjectConfiguration;
                foreach (Operation operation in Configuration.Operations) {
                    UpdateParents(operation);
                    ConnectProperties(operation);
                }
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }

            if (_pluginManager == null) {
                throw new ManagerInitializeException(nameof(_pluginManager));
            }

            return Configuration;
        }

        /// <summary>
        /// Removes the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(Tool tool) {
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            Operation operation = tool.Parent as Operation;
            if (operation != null) {
                Node outNode;
                operation.Items.TryRemove(tool.Id, out outNode);
            }
        }

        /// <summary>
        /// Removes the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Remove(Operation operation) {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            Configuration.Operations.Remove(operation);
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>
        /// Success of the operation.
        /// </returns>
        public override void Save(Stream stream) {
            try {
                if (_pluginManager == null) {
                    _pluginManager = CoreSystem.Instance.Plugins;
                }

                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), _pluginManager?.KnownTypes);
                serializer.WriteObject(stream, Configuration);
                stream.Flush();
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }
        }

        private void ConnectProperties(Node node) {
            foreach (Property item in node.Items.Values.OfType<Property>()) {
                if (!item.ConnectedId.Equals(Guid.Empty)) {
                    Property targetProperty = FindProperty(item.ConnectedId);
                    if (targetProperty == null) {
                        throw new NullReferenceException(nameof(targetProperty));
                    }

                    item.Connect(targetProperty);
                }

                ConnectProperties(item);
            }
        }

        private Device CreateDevice(Device dummyDevice) {
            Device device = _pluginManager.Items.Values.OfType<Device>().First(d => d.Fullname.Equals(dummyDevice.Fullname));
            Device copyDevice = device.Clone() as Device;
            copyDevice.Id = dummyDevice.Id;
            copyDevice.Items.Clear();

            foreach (Property property in dummyDevice.Items.Values.OfType<Property>()) {
                copyDevice.Items.TryAdd(property.Id, property);
            }

            return copyDevice;
        }

        private void UpdateParents(Operation operation) {
            foreach (Property childProperty in operation.Items.Values.OfType<Property>()) {
                childProperty.Parent = operation;
            }

            foreach (Tool childTool in operation.Items.Values.OfType<Tool>()) {
                childTool.Parent = operation;
            }
        }
    }
}