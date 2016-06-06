using ns.Base;
using ns.Base.Exceptions;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace ns.Core.Manager {

    public class ProjectManager : GenericConfigurationManager<ProjectConfiguration> {
        private static string _fileName = string.Empty;

        /// <summary>
        /// The file extension
        /// </summary>
        public const string FileExtension = ".xml";

        /// <summary>
        /// The default project file
        /// </summary>
        public const string DefaultProjectFile = "DefaultProject" + FileExtension;

        /// <summary>
        /// Gets the file filter.
        /// </summary>
        /// <value>
        /// The file filter.
        /// </value>
        public const string FileFilter = "Project File (*.xml) | *.xml";

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            Configuration = new ProjectConfiguration();
            return true;
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

            parent.CaptureDevice = device;
        }

        /// <summary>
        /// Creates the default project.
        /// </summary>
        public void CreateDefaultProject() {
            Configuration.Operations.Clear();
            Configuration.ProjectName.Value = "Default";

            PluginManager pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;

            if (pluginManager == null) {
                throw new ManagerInitialisationFailedException(nameof(pluginManager));
            }

            Operation operation = new Operation("Unknown Operation");

            Device device = pluginManager.Nodes.Find(n => n.Name.Equals("ns.Plugin.Base.ImageFileDevice")) as Device;
            if (device != null) {
                Add(device, operation);
            }

            Configuration.Operations.Add(operation);
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>
        /// The manager object. NULL if any error happend.
        /// </returns>
        /// <exception cref="ManagerInitialisationFailedException"></exception>
        /// <exception cref="ns.Base.Exceptions.PluginNotFoundException"></exception>
        /// <exception cref="ns.Base.Exceptions.DeviceInitialisationFailedException"></exception>
        public override ProjectConfiguration Load(Stream stream) {
            Configuration = base.Load(stream);

            PluginManager pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;

            if (pluginManager == null) {
                throw new ManagerInitialisationFailedException(nameof(pluginManager));
            }

            List<Device> devices = new List<Device>();

            foreach (Operation operation in Configuration.Operations) {
                Device device = operation.CaptureDevice;
                if (device != null) {
                    Device exactDevice = pluginManager.Nodes.Find(n => n.Fullname.Equals(device.Fullname)) as Device;
                    if (exactDevice == null) throw new PluginNotFoundException(device.Fullname);

                    exactDevice.Childs = device.Cache.Childs;
                    operation.CaptureDevice = exactDevice;
                    devices.Add(exactDevice);
                }
            }

            // Initialize all devices.
            foreach (Device device in devices) {
                if (!device.Initialize()) {
                    throw new DeviceInitialisationFailedException(device.Name);
                }
            }

            return Configuration;
        }
    }
}