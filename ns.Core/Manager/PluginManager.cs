using ns.Base;
using ns.Base.Attribute;
using ns.Base.Exceptions;
using ns.Base.Manager;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace ns.Core.Manager {

    public class PluginManager : NodeManager<Plugin> {
        private const string LIBRARY_EXTENSION = ".dll";
        private const string FACTORY_NAME = "Factory";

        private static string _pluginPath = AssemblyPath + "\\Plugins";

        private List<string> _fileList = new List<string>();
        private List<Assembly> _assemblyList = new List<Assembly>();
        private List<Plugin> _plugins = new List<Plugin>();
        private List<LibraryInformation> _libraryInformations = new List<LibraryInformation>();
        private ExtensionManager _extensionManager;

        /// <summary>
        /// Gets the path to the plugins.
        /// </summary>
        public string PluginPath {
            get {
                if (IsWebservice)
                    return Environment.GetEnvironmentVariable("NIDHOGGSTUDIO_BIN") + Path.DirectorySeparatorChar + "Plugins";
                else
                    return _pluginPath;
            }
        }

        /// <summary>
        /// Gets the library informations.
        /// </summary>
        /// <value>
        /// The library informations.
        /// </value>
        public List<LibraryInformation> LibraryInformations {
            get { return _libraryInformations; }
        }

        public List<Type> KnownTypes { get; } = new List<Type>();

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Could not initialize ToolManager!
        /// or
        /// Could not initialize DeviceManager!
        /// or
        /// Could not initialize ExtensionManager!
        /// or
        /// Could not get any plugins!
        /// </exception>
        public override bool Initialize() {
            try {
                Base.Log.Trace.WriteLine("Initialize PluginManager ...", TraceEventType.Information);
                _extensionManager = new ExtensionManager();

                if (!_extensionManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(ExtensionManager));

                CoreSystem.Managers.Add(_extensionManager);

                if (UpdatePlugins() == false)
                    throw new Exception("Could not get any plugins!");

                return true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Loads all Plugins.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool UpdatePlugins() {
            bool result = false;
            Base.Log.Trace.WriteLine("Updating Plugin list ... ", TraceEventType.Information);
            try {
                if (Directory.Exists(PluginPath) == false) {
                    Directory.CreateDirectory(PluginPath);
                    Base.Log.Trace.WriteLine("No plugins to load!", TraceEventType.Warning);
                } else {
                    _fileList.Clear();
                    _extensionManager.Nodes.Clear();

                    // Add the Plugins here!
                    Operation baseOperation = new Operation();
                    _plugins.Add(baseOperation);

                    foreach (string file in Directory.GetFiles(PluginPath)) {
                        if ((file.EndsWith(LIBRARY_EXTENSION) == false) || (_fileList.Contains(file) == true))
                            continue;
                        else
                            _fileList.Add(file);

                        Assembly assembly;

                        if ((assembly = _assemblyList.Find(a => a.Location == file)) == null) {
                            assembly = Assembly.LoadFile(file);
                            _assemblyList.Add(assembly);
                        }

                        // Searching for all [Visible] types as they may inherit [Plugin].
                        foreach (Type type in assembly.GetExportedTypes()) {
                            Visible visible = type.GetCustomAttribute(typeof(Visible)) as Visible;
                            DataContractAttribute dataContract = type.GetCustomAttribute(typeof(DataContractAttribute)) as DataContractAttribute;
                            if (visible == null || visible.IsVisible == false) continue;

                            if (dataContract == null) {
                                Base.Log.Trace.WriteLine(string.Format("Plugin {0} doesn't has the {1}! The plugin will not be used.", type.Name, nameof(DataContractAttribute)), TraceEventType.Warning);
                                continue;
                            }

                            object probalePlugin = assembly.CreateInstance(type.ToString());

                            Plugin plugin = probalePlugin as Plugin;
                            Extension extension = probalePlugin as Extension;
                            if (plugin == null) continue;

                            if (extension != null) _extensionManager.Add(extension);
                            else Add(plugin);

                            KnownTypes.Add(plugin.GetType());
                            _plugins.Add(plugin);
                        }
                    }
                }

                Base.Log.Trace.WriteLine("System contains " + (_plugins.Count - 1) + " Plugins:\n\t"
                    + Nodes.Count + " Plugin(s)\n\t"
                    + _extensionManager.Nodes.Count + " Extension(s)", TraceEventType.Verbose);

                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the plugin.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        /// <returns></returns>
        private bool ValidatePlugin(Plugin plugin) {
            bool result = false;

            try {
                string displayName = plugin.DisplayName;

                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <returns></returns>
        private bool ValidateTool(Tool tool) {
            bool result = false;

            try {
                if (ValidatePlugin(tool) == true) {
                    result = true;
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private bool ValidateOperation(Operation operation) {
            bool result = false;

            try {
                if (ValidatePlugin(operation) == true) {
                    result = true;
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        private bool ValidateDevice(Device device) {
            bool result = false;

            try {
                if (ValidatePlugin(device) == true) {
                    result = true;
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        /// <summary>
        /// Validates the extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        private bool ValidateExtension(Extension extension) {
            bool result = false;

            try {
                if (ValidatePlugin(extension) == true) {
                    result = true;
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }
    }
}