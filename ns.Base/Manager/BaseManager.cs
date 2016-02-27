using ns.Base.Event;
using ns.Base.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ns.Base.Manager {
    public class BaseManager : INotifyPropertyChanged {
        public delegate void NodeCollectionChangedHandler(object sender, NodeCollectionChangedEventArgs e);
        public event NodeCollectionChangedHandler NodeAddedEvent;
        public event NodeCollectionChangedHandler NodeRemovedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private const string APPLICATION_NAME = "Nidhogg Studio";
        private static string _assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string _documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + Path.DirectorySeparatorChar + APPLICATION_NAME + Path.DirectorySeparatorChar;

        private List<Node> _nodes = new List<Node>();

        private static bool _isWebservice = false;

        /// <summary>
        /// Gets the Typename.
        /// </summary>
        public string Name { get { return this.GetType().ToString(); } }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public static string ApplicationName {
            get { return APPLICATION_NAME; }
        }

        /// <summary>
        /// Gets the path to the application assembly.
        /// </summary>
        public static string AssemblyPath {
            get { 
                if(IsWebservice)
                    return Environment.GetEnvironmentVariable("NEUROSTUDIO_BIN");
                else
                    return _assemblyPath; 
            }
        }

        /// <summary>
        /// Gets the default documents path.
        /// </summary>
        public static string DocumentsPath {
            get { return _documentsPath; }
        }

        /// <summary>
        /// Gets the log path.
        /// </summary>
        public static string LogPath {
            get { return _documentsPath + "Log\\"; }
        }

        /// <summary>
        /// Gets the days the log files will be stored.
        /// </summary>
        public static uint DaysToKeepLogFiles {
            get { return 30; }
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>
        /// The nodes.
        /// </value>
        public List<Node> Nodes {
            get { return _nodes; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is webservice.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is webservice; otherwise, <c>false</c>.
        /// </value>
        public static bool IsWebservice {
            get { return _isWebservice; }
            set { _isWebservice = value; }
        }

        /// <summary>
        /// Initialize the instance of the manager.
        /// </summary>
        /// <returns></returns>
        public virtual bool Initialize() {
            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Finalize() {
            return true;
        }

        /// <summary>
        /// Called when [selection changed].
        /// </summary>
        /// <param name="selectedObject">The selected object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void OnSelectionChanged(object selectedObject) {
            selectedObject = null;
            throw new NotImplementedException();
        }

        public void OnNodeAdded(Node node) {
            if (this.NodeAddedEvent != null)
                this.NodeAddedEvent(this, new NodeCollectionChangedEventArgs(node));
        }

        public void OnNodeRemoved(Node node) {
            if (this.NodeRemovedEvent != null)
                this.NodeRemovedEvent(this, new NodeCollectionChangedEventArgs(node));
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Add(Node node) {
            if (!_nodes.Contains(node)) {
                _nodes.Add(node);
                OnNodeAdded(node);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        public virtual void AddRange(List<Node> nodes) {
            foreach (Node node in nodes) {
                Add(node);
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public virtual void Remove(Node node) {
            if (_nodes.Contains(node)) {
                _nodes.Remove(node);
                OnNodeRemoved(node);
            }
        }

        /// <summary>
        /// Load the manager from the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Success of the operation.</returns>
        public virtual object Load(string path) {
            try {
                object obj;
                using (FileStream stream = new FileStream(path, FileMode.Open)) {
                    obj = Load(stream);
                }
                return obj;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return null;
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
                if ((result = Save(ref memoryStream)) == true) {
                    using (FileStream stream = new FileStream(path, FileMode.Create)) {
                        memoryStream.Position = 0;
                        memoryStream.CopyTo(stream);
                        stream.Flush();
                    }
                }
                memoryStream.Close();
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Saves the manager to a MemoryStream.
        /// </summary>
        /// <param name="stream">Reference to the MemoryStream.</param>
        /// <returns>Success of the operation.</returns>
        public virtual bool Save(ref MemoryStream stream) {
            try {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stream, this);
                stream.Flush();
                return true;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return false;
            }
        }

        /// <summary>
        /// Loads the manager from a FileStream.
        /// </summary>
        /// <param name="stream">The FileStream.</param>
        /// <returns>The manager object. NULL if any error happend.</returns>
        public virtual object Load(FileStream stream) {
            try {
                object obj;
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                obj = serializer.Deserialize(stream);
                return obj;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                return null;
            }
        }

        public virtual bool Load() {
            throw new NotImplementedException("[Load] must be implemented!");
        }

        public virtual bool Save() {
            throw new NotImplementedException("[Load] must be implemented!");
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        public void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
