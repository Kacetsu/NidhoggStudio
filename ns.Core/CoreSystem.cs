using ns.Base.Exceptions;
using ns.Base.Extensions;
using ns.Base.Manager;
using ns.Core.Manager;
using ns.Core.Manager.ProjectBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ns.Core {

    /// <summary>
    /// The core, from here you should access any necessary system component.
    /// </summary>
    public class CoreSystem {
        private static CoreSystem _instance = new CoreSystem();
        private static List<BaseManager> _managers = new List<BaseManager>();
        private static Processor _processor;
        private static Base.Log.TraceListener _traceListener;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CoreSystem Instance => _instance;

        /// <summary>
        /// Gets the log listener.
        /// </summary>
        /// <value>
        /// The log listener.
        /// </value>
        public static Base.Log.TraceListener LogListener => _traceListener;

        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>
        /// The processor.
        /// </value>
        public static Processor Processor => _processor;

        /// <summary>
        /// Adds the manager.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public static void AddManager(BaseManager manager) {
            if (!_managers.Contains(manager)) {
                _managers.Add(manager);
            }
        }

        /// <summary>
        /// Finalizes the CoreSystem.
        /// </summary>
        /// <returns>True if successful.</returns>
        public static bool Finalize() {
            foreach (BaseManager manager in _managers)
                manager.Finalize();

            if (_traceListener != null)
                _traceListener.Close();

            return true;
        }

        /// <summary>
        /// Finds the manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T FindManager<T>(string name) where T : BaseManager {
            foreach (T manager in _managers.Where(m => m is T)) {
                if (manager.Name.Equals(name)) {
                    return manager;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds the manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FindManager<T>() where T : BaseManager { return (T)_managers.Find(m => m is T); }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public static bool Initialize() {
            bool result = false;

            try {
                CreateAssemblyResolver();

                CreateTraceListener();

                CreateManagers();

                _processor = new Processor();

                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }

            return result;
        }

        private static void CreateAssemblyResolver() {
            AppDomain.CurrentDomain.AssemblyResolve += delegate (object sender, ResolveEventArgs args) {
                // Find name (first argument)
                string assemblyName = args.Name.Substring(0, args.Name.IndexOf(','));
                try {
                    // Build the path to DLL and load it
                    // WARNING: The path has to be absolute otherwise it will raise an ArgumentException (security)
                    string libsPath = BaseManager.AssemblyPath + Path.DirectorySeparatorChar + "Libs" + Path.DirectorySeparatorChar + assemblyName + ".dll";
                    string basePath = BaseManager.AssemblyPath + Path.DirectorySeparatorChar + assemblyName + ".dll";

                    if (File.Exists(libsPath))
                        return Assembly.LoadFile(libsPath);
                    else if (File.Exists(basePath))
                        return Assembly.LoadFile(basePath);
                    else
                        return null;
                } catch (Exception ex) {
                    Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                    throw;
                }
            };
        }

        /// <summary>
        /// Creates the managers.
        /// </summary>
        /// <exception cref="ManagerInitialisationFailedException">
        /// </exception>
        private static void CreateManagers() {
            _managers.Clear();

            // Default managers must be added to the CoreSystem.
            ProjectManager projectManager = new ProjectManager();
            PropertyManager propertyManager = new PropertyManager();
            PluginManager pluginManager = new PluginManager();
            DataStorageManager dataStorageManager = new DataStorageManager();
            ProjectBoxManager projectBoxManager = new ProjectBoxManager();
            DeviceManager deviceManager = new DeviceManager();

            AddManager(projectManager);
            AddManager(pluginManager);
            AddManager(propertyManager);
            AddManager(dataStorageManager);
            AddManager(projectBoxManager);
            AddManager(deviceManager);

            if (!pluginManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

            if (!propertyManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

            if (!projectBoxManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

            if (!dataStorageManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(DataStorageManager));

            ExtensionManager extensionManager = _managers.Find(m => m.Name.Contains(nameof(ExtensionManager))) as ExtensionManager;

            if (!extensionManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(ExtensionManager));

            if (!deviceManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(DeviceManager));

            if (!projectManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));
        }

        /// <summary>
        /// Creates the trace listener.
        /// </summary>
        private static void CreateTraceListener() {
            _traceListener = new Base.Log.TraceListener(BaseManager.LogPath, BaseManager.DaysToKeepLogFiles);
            _traceListener.SetLoggingCategoties(new List<string> {
#if DEBUG
                    TraceEventType.Verbose.GetDescription(),
#endif
                    TraceEventType.Information.GetDescription(),
                    TraceEventType.Warning.GetDescription(),
                    TraceEventType.Error.GetDescription() });

            Base.Log.Trace.Listeners.Add(_traceListener);
            Base.Log.Trace.WriteLine("Initialize CoreSystem ...", TraceEventType.Information);
        }
    }
}