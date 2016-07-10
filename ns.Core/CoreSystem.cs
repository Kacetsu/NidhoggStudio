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
        private static Lazy<CoreSystem> _lazyInstance = new Lazy<CoreSystem>(() => new CoreSystem());
        private static List<BaseManager> _managers = new List<BaseManager>();
        private static Processor _processor;
        private static Base.Log.TraceListener _traceListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreSystem"/> class.
        /// </summary>
        public CoreSystem() {
            try {
                CreateAssemblyResolver();

                CreateTraceListener();

                CreateManagers();

                _processor = new Processor();

                IsInitialized = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CoreSystem Instance => _lazyInstance.Value;

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; } = false;

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
        public static void Close() {
            foreach (BaseManager manager in _managers) {
                manager.Close();
            }

            if (_traceListener != null) {
                _traceListener.Close();
            }
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
        /// <exception cref="ManagerInitializeException">
        /// </exception>
        private static void CreateManagers() {
            _managers.Clear();

            // Default managers must be added to the CoreSystem.
            ExtensionManager extensionManager = new ExtensionManager();
            AddManager(extensionManager);
            PluginManager pluginManager = new PluginManager();
            AddManager(pluginManager);
            PropertyManager propertyManager = new PropertyManager();
            AddManager(propertyManager);
            DataStorageManager dataStorageManager = new DataStorageManager();
            AddManager(dataStorageManager);
            DeviceManager deviceManager = new DeviceManager();
            AddManager(deviceManager);
            ProjectManager projectManager = new ProjectManager();
            AddManager(projectManager);
            ProjectBoxManager projectBoxManager = new ProjectBoxManager();
            AddManager(projectBoxManager);
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