using ns.Base.Exceptions;
using ns.Base.Extensions;
using ns.Base.Manager;
using ns.Core.Manager;
using ns.Core.Manager.ProjectBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ns.Core {

    /// <summary>
    /// The core, from here you should access any necessary system component.
    /// </summary>
    public class CoreSystem {
        private static CoreSystem _instance = new CoreSystem();
        private static List<BaseManager> _managers = new List<BaseManager>();
        private static Base.Log.TraceListener _traceListener;
        private static Processor _processor;
        private static Shell _shell;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CoreSystem Instance => _instance;

        /// <summary>
        /// Gets the managers.
        /// </summary>
        /// <value>
        /// The managers.
        /// </value>
        public static List<BaseManager> Managers => _managers;

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
        /// Gets the shell.
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public static Shell Shell => _shell;

        /// <summary>
        /// Initializes the specified is webservice.
        /// </summary>
        /// <param name="isWebservice">if set to <c>true</c> [is webservice].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Could not initialize ConfigurationManager!
        /// or
        /// Could not initialize PropertyManager!
        /// or
        /// Could not initialize PluginManager!
        /// or
        /// Could not initialize DisplayManager!
        /// or
        /// Could not initialize DataStorageManager!
        /// </exception>
        public static bool Initialize(bool isWebservice = false) {
            bool result = false;

            try {
                BaseManager.IsWebservice = isWebservice;

                //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + BaseManager.AssemblyPath + Path.DirectorySeparatorChar + "Libs");

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
                        throw ex;
                    }
                };

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
                _managers.Clear();

                // Default managers must be added to the CoreSystem.
                ProjectManager projectManager = new ProjectManager();
                PropertyManager propertyManager = new PropertyManager();
                PluginManager pluginManager = new PluginManager();
                DataStorageManager dataStorageManager = new DataStorageManager();
                ProjectBoxManager projectBoxManager = new ProjectBoxManager();

                if (!_managers.Contains(projectManager)) _managers.Add(projectManager);
                if (!_managers.Contains(pluginManager)) _managers.Add(pluginManager);
                if (!_managers.Contains(propertyManager)) _managers.Add(propertyManager);
                if (!_managers.Contains(dataStorageManager)) _managers.Add(dataStorageManager);
                if (!_managers.Contains(projectBoxManager)) _managers.Add(projectBoxManager);

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                if (!pluginManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

                if (!propertyManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

                if (!projectBoxManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

                if (!dataStorageManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(DataStorageManager));

                ExtensionManager extensionManager = Managers.Find(m => m.Name.Contains(nameof(ExtensionManager))) as ExtensionManager;

                if (!extensionManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(ExtensionManager));

                if (!projectManager.Initialize()) throw new ManagerInitialisationFailedException(nameof(PluginManager));

                _processor = new Processor();
                _shell = new Shell();
                _shell.Initialize();

                result = true;
            } catch (Exception ex) {
                Base.Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This method is needed to let the system know where the plugins are.
        /// @warning If this method is not used, the BinaryFormatter will throw an exception as it will now find the assembly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            Assembly result = null;
            string shortAssemblyName = args.Name.Split(',')[0];
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies) {
                if (shortAssemblyName == assembly.FullName.Split(',')[0]) {
                    result = assembly;
                    break;
                }
            }

            return result;
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
    }
}