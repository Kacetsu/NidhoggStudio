using ns.Base;
using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Base.Plugins.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Core.Manager {

    public class PluginManager : NodeManager<Plugin>, INodeManager<Plugin> {
        private const string FACTORY_NAME = "Factory";
        private const string LIBRARY_EXTENSION = ".dll";
        private static string _pluginPath = AssemblyPath + "\\Plugins";

        private List<Assembly> _assemblyList = new List<Assembly>();
        private List<string> _fileList = new List<string>();
        private List<LibraryInformation> _libraryInformations = new List<LibraryInformation>();
        private List<Plugin> _plugins = new List<Plugin>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">Could not get any plugins!</exception>
        public PluginManager([CallerMemberName] string name = null) : base(name) {
            try {
                Base.Log.Trace.WriteLine("Initialize PluginManager ...", TraceEventType.Information);

                if (!UpdatePlugins()) {
                    throw new Exception("Could not get any plugins!");
                }
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the known types.
        /// </summary>
        /// <value>
        /// The known types.
        /// </value>
        public List<Type> KnownTypes { get; } = new List<Type>();

        /// <summary>
        /// Gets the library informations.
        /// </summary>
        /// <value>
        /// The library informations.
        /// </value>
        public List<LibraryInformation> LibraryInformations => _libraryInformations;

        /// <summary>
        /// Gets the path to the plugins.
        /// </summary>
        public string PluginPath => _pluginPath;

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
                            VisibleAttribute visible = type.GetCustomAttribute(typeof(VisibleAttribute)) as VisibleAttribute;
                            DataContractAttribute dataContract = type.GetCustomAttribute(typeof(DataContractAttribute)) as DataContractAttribute;
                            if (visible == null || visible.IsVisible == false) continue;

                            if (dataContract == null) {
                                Base.Log.Trace.WriteLine(string.Format("Plugin {0} doesn't has the {1}! The plugin will not be used.", type.Name, nameof(DataContractAttribute)), TraceEventType.Warning);
                                continue;
                            }

                            object probalePlugin = assembly.CreateInstance(type.ToString());

                            Plugin plugin = probalePlugin as Plugin;
                            if (plugin == null) continue;

                            Device device = plugin as Device;
                            Tool tool = plugin as Tool;
                            Operation operation = plugin as Operation;
                            Factory factory = plugin as Factory;

                            if (device != null && !ValidateDevice(device)) {
                                continue;
                            } else if (tool != null && !ValidateTool(tool)) {
                                continue;
                            } else if (operation != null && !ValidateOperation(operation)) {
                                continue;
                            } else if (factory != null) {
                                factory.Initialize();
                                foreach (Device d in factory.Items.Values.Where(i => i is Device).Cast<Device>()) {
                                    if (!ValidateDevice(d)) {
                                        continue;
                                    }

                                    Add(d);
                                    if (!KnownTypes.Contains(d.GetType())) {
                                        KnownTypes.Add(d.GetType());
                                    }
                                    _plugins.Add(d);
                                }

                                continue;
                            } else if (device == null && tool == null && operation == null) {
                                Base.Log.Trace.WriteLine(string.Format("Unknown plugin {0}!", plugin.GetType().Name), TraceEventType.Error);
                                continue;
                            }

                            Add(plugin);

                            KnownTypes.Add(plugin.GetType());
                            _plugins.Add(plugin);
                        }
                    }
                }

                result = true;
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
    }
}