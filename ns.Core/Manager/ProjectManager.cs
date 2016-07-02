using ns.Base.Exceptions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                throw new ManagerInitialisationFailedException(nameof(pluginManager));
            }

            operation.AddDeviceList(pluginManager.Nodes.Where(d => d is Device).Cast<Device>().ToList());
            Configuration.Operations.Add(operation);
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

            parent.AddChild(tool);
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

            if (deviceManager == null) throw new ManagerInitialisationFailedException(nameof(deviceManager));

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
                throw new ManagerInitialisationFailedException(nameof(pluginManager));
            }

            Operation operation = new Operation("Unknown Operation");

            Device device = pluginManager.Nodes.Find(n => n.Name.Equals("ns.Plugin.Base.ImageFileDevice")) as Device;
            if (device != null) {
                Add(device, operation);
            }

            Add(operation);
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
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            Configuration = new ProjectConfiguration();
            CreateDefaultProject();
            return true;
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>
        /// The manager object. NULL if any error happend.
        /// </returns>
        /// <exception cref="ManagerInitialisationFailedException"></exception>
        /// <exception cref="PluginNotFoundException"></exception>
        /// <exception cref="DeviceInitialisationFailedException"></exception>
        public override ProjectConfiguration Load(Stream stream) {
            PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();

            try {
                stream.Position = 0;
                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), pluginManager?.KnownTypes);
                Configuration = serializer.ReadObject(stream) as ProjectConfiguration;
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }

            if (pluginManager == null) {
                throw new ManagerInitialisationFailedException(nameof(pluginManager));
            }

            foreach (Operation operation in Configuration.Operations) {
                Device device = operation.CaptureDevice;
                if (device != null) {
                    if (!device.Initialize()) {
                        throw new DeviceInitialisationFailedException(device.Name);
                    }
                }
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
        public override bool Save(Stream stream) {
            try {
                PluginManager pluginManager = CoreSystem.FindManager<PluginManager>();
                DataContractSerializer serializer = new DataContractSerializer(typeof(ProjectConfiguration), pluginManager?.KnownTypes);
                serializer.WriteObject(stream, Configuration);
                stream.Flush();
                return true;
            } catch (SerializationException ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }
    }
}