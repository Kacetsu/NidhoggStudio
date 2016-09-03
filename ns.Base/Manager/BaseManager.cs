using System;
using System.IO;
using System.Reflection;

namespace ns.Base.Manager {

    public class BaseManager : NotifiableObject, IManager {
        private const string APPLICATION_NAME = "Nidhogg Studio";
        private static string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + Path.DirectorySeparatorChar + APPLICATION_NAME + Path.DirectorySeparatorChar;

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

        /// <summary>
        /// Gets the Typename.
        /// </summary>
        public string Name => GetType().ToString();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() {
        }
    }
}