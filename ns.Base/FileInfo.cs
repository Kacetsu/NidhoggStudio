using System;
using System.Diagnostics;
using System.IO;

namespace ns.Base {

    public class FileInfo {

        public static void CopyDirectory(string sourcePath, string destinationPath) {
            Directory.CreateDirectory(destinationPath);
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }

        public static bool CopyFile(string source, string destination) {
            if (!File.Exists(source)) return false;
            try {
                File.Copy(source, destination, true);
                return true;
            } catch (Exception ex) {
                Log.Trace.WriteLine(ex, TraceEventType.Error);
                return false;
            }
        }

        public static bool CreateDirectory(string path) {
            DirectoryInfo info = Directory.CreateDirectory(path);
            return info.Exists;
        }

        public static string[] GetDirectories(string path) {
            return Directory.GetDirectories(path);
        }

        public static bool IsDirectoryEmpty(string path) {
            if (!Directory.Exists(path)) return true;
            return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
        }
    }
}