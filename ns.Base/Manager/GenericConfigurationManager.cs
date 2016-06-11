using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base.Manager {

    public class GenericConfigurationManager<T> : BaseManager {

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public T Configuration { get; set; }

        /// <summary>
        /// Load the manager from the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Success of the operation.</returns>
        public virtual bool Load(string path) {
            try {
                T obj;
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    obj = Load(stream);
                }
                Configuration = obj;
                return true;
            } catch (SerializationException ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>The manager object. NULL if any error happend.</returns>
        public virtual T Load(Stream stream) {
            try {
                T obj;
                stream.Position = 0;
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                obj = (T)serializer.ReadObject(stream);
                return obj;
            } catch (SerializationException ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return default(T);
            }
        }

        /// <summary>
        /// Saves the manager from the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Success of the operation.</returns>
        public virtual bool Save(string path) {
            bool result = false;
            try {
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                MemoryStream memoryStream = new MemoryStream();
                if ((result = Save(memoryStream)) == true) {
                    using (FileStream stream = new FileStream(path, FileMode.Create)) {
                        memoryStream.Position = 0;
                        memoryStream.CopyTo(stream);
                        stream.Flush();
                    }
                }
                memoryStream.Close();
            } catch (Exception ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>Success of the operation.</returns>
        public virtual bool Save(Stream stream) {
            try {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, Configuration);
                stream.Flush();
                return true;
            } catch (SerializationException ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                return false;
            }
        }

        public virtual bool Load() {
            throw new NotImplementedException("[Load] must be implemented!");
        }

        public virtual bool Save() {
            throw new NotImplementedException("[Load] must be implemented!");
        }
    }
}