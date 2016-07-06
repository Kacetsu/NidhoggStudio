using ns.Base.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Manager {

    public class GenericConfigurationManager<T> : BaseManager, IGenericConfigurationManager<T> where T : IBaseConfiguration {

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericConfigurationManager{T}"/> class.
        /// </summary>
        public GenericConfigurationManager() : base() {
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public T Configuration { get; set; }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public virtual void Load(string path) {
            try {
                T obj;
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    obj = Load(stream);
                }
                Configuration = obj;
            } catch (SerializationException ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
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

        public virtual void Load() {
            throw new NotImplementedException("[Load] must be implemented!");
        }

        /// <summary>
        /// Saves the manager from the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Success of the operation.</returns>
        public virtual void Save(string path) {
            if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            using (MemoryStream memoryStream = new MemoryStream()) {
                Configuration.FileName.Value = path;
                Save(memoryStream);
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(stream);
                    stream.Flush();
                }
            }
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>Success of the operation.</returns>
        public virtual void Save(Stream stream) {
            try {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream, Configuration);
                stream.Flush();
            } catch (SerializationException ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
                throw;
            }
        }

        public virtual void Save() {
            throw new NotImplementedException("[Load] must be implemented!");
        }
    }
}