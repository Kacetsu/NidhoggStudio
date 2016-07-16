using ns.Base;
using ns.Base.Exceptions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManager"/> class.
        /// </summary>
        public ProjectManager() : base() {
            Configuration = new ProjectConfiguration();
            CreateDefaultProject();
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

            PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();

            if (pluginManager == null) {
                throw new ManagerInitializeException(nameof(pluginManager));
            }

            operation.AddDeviceList(pluginManager.Nodes.Where(d => d is Device).Cast<Device>().ToList());

            if (CoreSystem.Processor?.State == ProcessorState.Running) {
                operation.Initialize();
            }

            Configuration.Operations.Add(operation);

            foreach (Property property in operation.Childs.Where(p => p is Property && (p as Property).CanAutoConnect)) {
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

            if (parent.Childs.Contains(tool)) throw new ToolAlreadyExistsException(tool.Name);

            if (CoreSystem.Processor?.State == ProcessorState.Running) {
                tool.Initialize();
            }

            parent.AddChild(tool);

            foreach (Property property in tool.Childs.Where(p => p is Property && (p as Property).CanAutoConnect)) {
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

            DeviceManager deviceManager = CoreSystem.FindManager<DeviceManager>();

            if (deviceManager == null) throw new ManagerInitializeException(nameof(deviceManager));

            deviceManager.Add(device);

            parent.CaptureDevice = device;
        }

        /// <summary>
        /// Creates the default project.
        /// </summary>
        public void CreateDefaultProject() {
            Configuration.Operations.Clear();
            Configuration.Name.Value = "Default";

            PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();

            if (pluginManager == null) {
                throw new ManagerInitializeException(nameof(pluginManager));
            }

            Operation operation = new Operation("Unknown Operation");

            Device device = pluginManager.Nodes.Find(n => n.Name.Equals("ns.Plugin.Base.ImageFileDevice")) as Device;
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
            if (property.IsOutput) return result;

            Operation operation = FindOperation(property);
            Tool tool = FindTool(property);

            foreach (Node childNode in operation.Childs) {
                Tool childTool = childNode as Tool;
                Property childProperty = childNode as Property;

                if (childTool != null) {
                    if (childTool.Equals(tool)) break;

                    foreach (Property p in childTool.Childs.Where(p => p is Property)) {
                        if (p.GetType().Equals(property.GetType()) && p.IsOutput) {
                            result.Add(p);
                        }
                    }
                } else if (childProperty != null && childProperty.GetType().Equals(property.GetType()) && childProperty.IsOutput) {
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
                foreach (Node node in operation.Childs) {
                    Tool childTool = node as Tool;
                    Property childProperty = node as Property;

                    if (childTool != null) {
                        foreach (Property p in childTool.Childs.Where(p => p is Property)) {
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
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public Property FindProperty(string uid) {
            Property property = null;
            foreach (Operation operation in Configuration.Operations) {
                property = operation.Childs.Find(p => p.UID.Equals(uid)) as Property;
                if (property != null) {
                    break;
                }
                foreach (Tool tool in operation.Childs.Where(t => t is Tool)) {
                    property = tool.Childs.Find(p => p.UID.Equals(uid)) as Property;
                    if (property != null) {
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
            foreach (Tool tool in operation.Childs.Where(t => t is Tool)) {
                if (tool.Childs.Contains(property)) return tool;
            }

            return null;
        }

        /// <summary>
        /// Finds the tool.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public Tool FindTool(string uid) {
            foreach (Operation operation in Configuration.Operations) {
                Tool tool = operation.Childs.Find(t => t.UID.Equals(uid)) as Tool;
                return tool;
            }

            return null;
        }

        /// <summary>
        /// Initializes the operations.
        /// </summary>
        /// <exception cref="OperationInitializeException"></exception>
        public void InitializeOperations() {
            foreach (Operation operation in Configuration.Operations) {
                if (!operation.Initialize()) {
                    throw new OperationInitializeException(operation.Name);
                }
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
            PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();

            try {
                stream.Position = 0;
                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), pluginManager?.KnownTypes);
                Configuration = serializer.ReadObject(stream) as ProjectConfiguration;
                foreach (Operation operation in Configuration.Operations) {
                    UpdateParents(operation);
                }
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }

            if (pluginManager == null) {
                throw new ManagerInitializeException(nameof(pluginManager));
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
                operation.Childs.Remove(tool);
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
                PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();
                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), pluginManager?.KnownTypes);
                serializer.WriteObject(stream, Configuration);
                stream.Flush();
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }
        }

        private void UpdateParents(Operation operation) {
            foreach (Property childProperty in operation.Childs.Where(p => p is Property)) {
                childProperty.Parent = operation;
            }

            foreach (Tool childTool in operation.Childs.Where(t => t is Tool)) {
                childTool.Parent = operation;
            }
        }
    }
}