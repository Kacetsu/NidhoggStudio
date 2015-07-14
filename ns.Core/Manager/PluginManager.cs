using ns.Base.Attribute;
using ns.Base.Log;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Manager;
using ns.Base;

namespace ns.Core.Manager {
    public class PluginManager : BaseManager {

        private const string LIBRARY_EXTENSION = ".dll";
        private const string FACTORY_NAME = "Factory";

        private static string _pluginPath = BaseManager.AssemblyPath + "\\Plugins";

        private List<string> _fileList = new List<string>();
        private List<Assembly> _assemblyList = new List<Assembly>();
        private List<Plugin> _plugins = new List<Plugin>();
        private List<LibraryInformation> _libraryInformations = new List<LibraryInformation>();

        /// <summary>
        /// Gets the path to the plugins.
        /// </summary>
        public string PluginPath {
            get {
                if (IsWebservice)
                    return Environment.GetEnvironmentVariable("NEUROSTUDIO_BIN") + Path.DirectorySeparatorChar + "Plugins";
                else
                    return _pluginPath; 
            }
        }

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        public List<Plugin> Plugins {
            get {
                return _plugins;
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
                Trace.WriteLine("Initialize PluginManager ...", LogCategory.Info);
                ToolManager toolManager = new ToolManager();
                DeviceManager deviceManager = new DeviceManager();
                ExtensionManager extensionManager = new ExtensionManager();

                if (toolManager.Initialize() == false)
                    throw new Exception("Could not initialize ToolManager!");

                if(deviceManager.Initialize() == false)
                    throw new Exception("Could not initialize DeviceManager!");

                if(extensionManager.Initialize() == false)
                    throw new Exception("Could not initialize ExtensionManager!");

                CoreSystem.Managers.Add(toolManager);
                CoreSystem.Managers.Add(deviceManager);
                CoreSystem.Managers.Add(extensionManager);

                if (UpdatePlugins() == false)
                    throw new Exception("Could not get any plugins!");

                return true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        /// <summary>
        /// Loads all Plugins.
        /// </summary>
        /// <returns>Success of the operation.</returns>
        public bool UpdatePlugins() {
            bool result = false;
            Trace.WriteLine("Updating Plugin list ... ", LogCategory.Info);
            try {
                ToolManager toolManager = (ToolManager)CoreSystem.Managers.Find(pm => pm.GetType() == typeof(ToolManager));
                DeviceManager deviceManager = (DeviceManager)CoreSystem.Managers.Find(pm => pm.GetType() == typeof(DeviceManager));
                ExtensionManager extensionManager = (ExtensionManager)CoreSystem.Managers.Find(pm => pm.GetType() == typeof(ExtensionManager));

                if (Directory.Exists(PluginPath) == false) {
                    Directory.CreateDirectory(PluginPath);
                    Trace.WriteLine("No plugins to load!", LogCategory.Warning);
                } else {
                    _fileList.Clear();
                    toolManager.Plugins.Clear();
                    deviceManager.Plugins.Clear();
                    extensionManager.Plugins.Clear();

                    // Add the Plugins here!
                    Operation baseOperation = new Operation();
                    _plugins.Add(baseOperation);
                    toolManager.Plugins.Add(baseOperation);

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
                            Visible visible = (Visible)type.GetCustomAttribute(typeof(Visible));
                            if (visible == null || visible.IsVisible == false)
                                continue;
                            object probalePlugin = assembly.CreateInstance(type.ToString());

                            Plugin plugin = null;
                            if (probalePlugin is Tool) {
                                plugin = probalePlugin as Tool;
                                if (ValidateTool(plugin as Tool) == false)
                                    plugin = null;

                                toolManager.Plugins.Add(plugin);
                            } else if(probalePlugin is Operation) {
                                plugin = probalePlugin as Operation;
                                if (ValidateOperation(plugin as Operation) == false)
                                    plugin = null;

                                toolManager.Plugins.Add(plugin);
                            } else if (probalePlugin is Device) {
                                plugin = probalePlugin as Device;
                                if (ValidateDevice(plugin as Device) == false)
                                    plugin = null;

                                deviceManager.Plugins.Add(plugin);
                            } else if (probalePlugin is Extension) {
                                plugin = probalePlugin as Extension;
                                if(ValidateExtension(plugin as Extension) == false)
                                    plugin = null;

                                extensionManager.Plugins.Add(plugin);
                            } else if (probalePlugin is LibraryInformation) {
                                LibraryInformation libraryInformation = probalePlugin as LibraryInformation;
                                _libraryInformations.Add(libraryInformation);
                            }

                            if (plugin == null)
                                continue;

                            _plugins.Add(plugin);
                        }

                    }
                }

                Trace.WriteLine("System contains " + _plugins.Count + " Plugins:\n\t"
                    + toolManager.Plugins.Count + " Tool(s)\n\t" 
                    + deviceManager.Plugins.Count + " Device(s)\n\t" 
                    + extensionManager.Plugins.Count + " Extension(s)", LogCategory.Debug);

                result = true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            return result;
        }

    }
}
