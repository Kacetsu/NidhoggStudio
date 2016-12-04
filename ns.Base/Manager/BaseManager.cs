using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ns.Base.Manager {

    public abstract class BaseManager : Node, IManager {
        private const string APPLICATION_NAME = "Nidhogg Studio";
        private static string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + Path.DirectorySeparatorChar + APPLICATION_NAME + Path.DirectorySeparatorChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseManager"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public BaseManager([CallerMemberName] string name = null) : base() {
            Name = name;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public static string ApplicationName => APPLICATION_NAME;

        /// <summary>
        /// Gets the path to the application assembly.
        /// </summary>
        public static string AssemblyPath => _assemblyPath;

        /// <summary>
        /// Gets the days the log files will be stored.
        /// </summary>
        public static int DaysToKeepLogFiles => 30;

        /// <summary>
        /// Gets the default documents path.
        /// </summary>
        public static string DocumentsPath => _documentsPath;

        /// <summary>
        /// Gets the log path.
        /// </summary>
        public static string LogPath => _documentsPath + "Log\\";
    }
}