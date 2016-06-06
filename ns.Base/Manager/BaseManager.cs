using System;
using System.IO;
using System.Reflection;

namespace ns.Base.Manager {

    public class BaseManager : NotifiableObject {
        private const string APPLICATION_NAME = "Nidhogg Studio";
        private static string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + Path.DirectorySeparatorChar + APPLICATION_NAME + Path.DirectorySeparatorChar;

        /// <summary>
        /// Gets the Typename.
        /// </summary>
        public string Name => GetType().ToString();

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public static string ApplicationName => APPLICATION_NAME;

        /// <summary>
        /// Gets the path to the application assembly.
        /// </summary>
        public static string AssemblyPath {
            get {
                if (IsWebservice)
                    return Environment.GetEnvironmentVariable("NEUROSTUDIO_BIN");
                else
                    return _assemblyPath;
            }
        }

        /// <summary>
        /// Gets the default documents path.
        /// </summary>
        public static string DocumentsPath => _documentsPath;

        /// <summary>
        /// Gets the log path.
        /// </summary>
        public static string LogPath => _documentsPath + "Log\\";

        /// <summary>
        /// Gets the days the log files will be stored.
        /// </summary>
        public static uint DaysToKeepLogFiles => 30;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is webservice.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is webservice; otherwise, <c>false</c>.
        /// </value>
        public static bool IsWebservice { get; set; } = false;

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize() => true;

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Finalize() => true;
    }
}