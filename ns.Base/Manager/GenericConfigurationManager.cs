using ns.Base.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;

namespace ns.Base.Manager {

    public abstract class GenericConfigurationManager<T> : BaseManager, IGenericConfigurationManager<T> where T : IBaseConfiguration {

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericConfigurationManager{T}"/> class.
        /// </summary>
        public GenericConfigurationManager([CallerMemberName] string name = null) : base(name) {
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
            if (stream == null) throw new ArgumentNullException(nameof(stream));

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
                memoryStream.Position = 0;
                using (XmlReader xmlReader = XmlReader.Create(memoryStream)) {
                    using (XmlWriter xmlWriter = XmlWriter.Create(path, new XmlWriterSettings() { Indent = true })) {
                        xmlWriter.WriteNode(xmlReader, false);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>Success of the operation.</returns>
        public virtual void Save(Stream stream) {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

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