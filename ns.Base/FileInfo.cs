using System;
using System.Diagnostics;
using System.IO;

namespace ns.Base {

    public static class FileInfo {

        /// <summary>
        /// Copies the directory.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        public static void CopyDirectory(string sourcePath, string destinationPath) {
            Directory.CreateDirectory(destinationPath);
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static void CopyFile(string source, string destination) {
            if (!File.Exists(source)) throw new FileNotFoundException(source);

            try {
                File.Copy(source, destination, true);
            } catch (Exception ex) {
                Log.Trace.WriteLine(ex, TraceEventType.Error);
                throw;
            }
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool CreateDirectory(string path) {
            DirectoryInfo info = Directory.CreateDirectory(path);
            return info.Exists;
        }

        /// <summary>
        /// Gets the directories.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string[] GetDirectories(string path) {
            return Directory.GetDirectories(path);
        }

        /// <summary>
        /// Determines whether [is directory empty] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static bool IsDirectoryEmpty(string path) {
            if (!Directory.Exists(path)) return true;
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }
    }
}