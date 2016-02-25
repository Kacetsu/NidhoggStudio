using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Manager;
using ns.Core.Manager;
using ns.ProjectBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ns.Core {

    /// <summary>
    /// The core, from here you should access any necessary system component.
    /// </summary>
    public class CoreSystem {

        private static CoreSystem _instance = new CoreSystem();
        private static List<BaseManager> _managers = new List<BaseManager>();
        private static TraceListener _traceListener;
        private static Processor _processor;
        private static Shell _shell;


        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CoreSystem Instance { get { return _instance; } }


        /// <summary>
        /// Gets the managers.
        /// </summary>
        /// <value>
        /// The managers.
        /// </value>
        public static List<BaseManager> Managers {
            get { return _managers; }
        }


        /// <summary>
        /// Gets the log listener.
        /// </summary>
        /// <value>
        /// The log listener.
        /// </value>
        public static TraceListener LogListener {
            get { return _traceListener; }
        }


        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>
        /// The processor.
        /// </value>
        public static Processor Processor {
            get { return _processor; }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        /// <value>
        /// The shell.
        /// </value>
        public static Shell Shell {
            get { return _shell; }
        }

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
        public static bool Initialize(bool isWebservice) {
            bool result = false;

            try {
                BaseManager.IsWebservice = isWebservice;

                //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + BaseManager.AssemblyPath + Path.DirectorySeparatorChar + "Libs");

                AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args) {
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
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                        throw ex;
                    }
                };

                _traceListener = new TraceListener(BaseManager.LogPath, BaseManager.DaysToKeepLogFiles);
                _traceListener.SetLoggingCategoties(new List<string> { 
#if DEBUG
                    LogCategory.Debug.GetDescription(), 
#endif
                    LogCategory.Info.GetDescription(), 
                    LogCategory.Warning.GetDescription(), 
                    LogCategory.Error.GetDescription() });
                System.Diagnostics.Trace.Listeners.Add(_traceListener);
                Trace.WriteLine("Initialize CoreSystem ...", LogCategory.Info);
                _managers.Clear();

                // Default managers must be added to the CoreSystem.
                ProjectManager projectManager = new ProjectManager();
                PropertyManager propertyManager = new PropertyManager();
                PluginManager pluginManager = new PluginManager();
                DisplayManager displayManager = new DisplayManager();
                DataStorageManager dataStorageManager = new DataStorageManager();
                ProjectBoxManager projectBoxManager = new ProjectBoxManager();

                _managers.Add(projectManager);
                _managers.Add(propertyManager);
                _managers.Add(pluginManager);
                _managers.Add(displayManager);
                _managers.Add(dataStorageManager);
                _managers.Add(projectBoxManager);

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                if (pluginManager.Initialize() == false)
                    throw new Exception("Could not initialize PluginManager!");

                if (propertyManager.Initialize() == false)
                    throw new Exception("Could not initialize PropertyManager!");

                if(projectBoxManager.Initialize() == false)
                    throw new Exception("Could not initialize ProjectBoxManager!");

                if (projectManager.Initialize() == false)
                    throw new Exception("Could not initialize ProjectManager!");

                _traceListener.SetLoggingCategoties(projectManager.Configuration.LogConfiguration.Categories);

                if (displayManager.Initialize() == false)
                    throw new Exception("Could not initialize DisplayManager!");

                if (dataStorageManager.Initialize() == false)
                    throw new Exception("Could not initialize DataStorageManager!");

                ExtensionManager extensionManager = CoreSystem.Managers.Find(m => m.Name.Contains("ExtensionManager")) as ExtensionManager;

                if (extensionManager.Initialize() == false)
                    throw new Exception("Could not initialize ExtensionManager!");

                _processor = new Core.Processor();
                _shell = new Core.Shell();
                _shell.Initialize();
                result = true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
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

            if(_traceListener != null)
                _traceListener.Close();

            return true;
        }
    }
}
