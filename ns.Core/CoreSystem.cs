using ns.Base;
using ns.Base.Extensions;
using ns.Base.Manager;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ns.Core {

    /// <summary>
    /// The core, from here you should access any necessary system component.
    /// </summary>
    public sealed class CoreSystem : Node {
        private static Lazy<CoreSystem> _lazyInstance = new Lazy<CoreSystem>(() => new CoreSystem());
        private static Base.Log.TraceListener _traceListener;

        /// <summary>
        /// Gets the data storage.
        /// </summary>
        /// <value>
        /// The data storage.
        /// </value>
        public DataStorageManager DataStorage => FindOrAdd<DataStorageManager>();

        /// <summary>
        /// Gets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        public DeviceManager Devices => FindOrAdd<DeviceManager>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CoreSystem Instance => _lazyInstance.Value;

        /// <summary>
        /// Gets the log listener.
        /// </summary>
        /// <value>
        /// The log listener.
        /// </value>
        public static Base.Log.TraceListener LogListener => _traceListener;

        /// <summary>
        /// Gets the plugins.
        /// </summary>
        /// <value>
        /// The plugins.
        /// </value>
        public PluginManager Plugins => FindOrAdd<PluginManager>();

        /// <summary>
        /// Gets the processor.
        /// </summary>
        /// <value>
        /// The processor.
        /// </value>
        public Processor Processor => FindOrAdd<Processor>();

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public ProjectManager Project => FindOrAdd<ProjectManager>();

        /// <summary>
        /// Gets the project box.
        /// </summary>
        /// <value>
        /// The project box.
        /// </value>
        public ProjectBoxManager ProjectBox => FindOrAdd<ProjectBoxManager>();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public PropertyManager Properties => FindOrAdd<PropertyManager>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            CreateAssemblyResolver();
            CreateTraceListener();
            Plugins.Initialize();
            Devices.Initialize();
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