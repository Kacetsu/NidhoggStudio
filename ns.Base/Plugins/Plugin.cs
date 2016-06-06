using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
    /// </summary>
    [Serializable]
    public class Plugin : Node, IPlugin, ICloneable, IXmlSerializable {
        private string _version = string.Empty;
        private string _assemblyFile = string.Empty;
        private string _displayName = string.Empty;
        private PluginStatus _status = PluginStatus.Unknown;

        /// <summary>
        /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
        /// Creates additional to the BaseNode the following fields: AssemblyFile and Version.
        /// </summary>
        public Plugin() : base() {
            Cache = new Cache();
        }

        /// <summary>
        /// Gets or sets the AssemblyFile.
        /// Used to find the correct Plugin while loading the CoreSystem / PluginManager.
        /// </summary>
        [XmlAttribute]
        public string AssemblyFile {
            get {
                if (string.IsNullOrEmpty(_assemblyFile)) {
                    Assembly assembly = Assembly.GetAssembly(this.GetType());
                    _assemblyFile = Path.GetFileNameWithoutExtension(assembly.Location);
                }
                return _assemblyFile;
            }
            set { _assemblyFile = value; }
        }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        [XmlAttribute]
        public string Version {
            get {
                if (string.IsNullOrEmpty(_version)) {
                    Assembly assembly = Assembly.GetAssembly(this.GetType());
                    AssemblyFileVersionAttribute attribute = (AssemblyFileVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
                    _version = attribute.Version;
                }
                return _version;
            }
            set { _version = value; }
        }

        /// <summary>
        /// Gets the DisplayName.
        /// The DisplayName is used for the Application User to visualize a human readable Name.
        /// </summary>
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(_displayName))
                    _displayName = this.GetType().Name;
                return _displayName;
            }
            set {
                if (!_displayName.Equals(value)) {
                    _displayName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public virtual string Description {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public PluginStatus Status {
            get { return _status; }
            set {
                _status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the NodeCache.
        /// </summary>
        public Cache Cache { get; set; }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public virtual bool PreRun() {
            this.OnStarted();
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>Success of the Operation.</returns>
        public virtual bool Run() {
            return true;
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public virtual bool PostRun() {
            this.OnFinished();
            return true;
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Terminate() {
            return true;
        }

        /// <summary>
        /// Runs the childs.
        /// </summary>
        /// <returns></returns>
        public virtual bool RunChilds() {
            bool result = true;
            lock (Childs) {
                foreach (Plugin child in Childs.Where(p => p is Plugin)) {
                    if (child.PreRun() == false) {
                        Log.Trace.WriteLine("Plugin " + child.Name + " pre run failed!", TraceEventType.Error);
                        result = false;
                    } else if (child.Run() == false) {
                        Log.Trace.WriteLine("Plugin " + child.Name + " run failed!", TraceEventType.Error);
                        result = false;
                    } else if (child.PostRun() == false) {
                        Log.Trace.WriteLine("Plugin " + child.Name + " post run failed!", TraceEventType.Error);
                        result = false;
                    }

                    foreach (Property property in child.Childs) {
                        ITolerance<object> tolerancProperty = property as ITolerance<object>;
                        if (tolerancProperty?.IsToleranceEnabled == false) continue;

                        result = tolerancProperty.InTolerance;
                    }

                    if (!result) break;
                }

                if (!result) {
                    // Set Status flag.
                    Status = PluginStatus.Failed;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the first property with the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Property GetProperty(string name) {
            Property result = null;

            foreach (Property property in Childs.Where(p => p is Property)) {
                if (property.Name == name) {
                    result = property;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the first property with the matching type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Property GetProperty(Type type) {
            Property result = null;

            foreach (Property property in Childs.Where(p => p is Property)) {
                if (property.GetType() == type) {
                    result = property;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Called when [started].
        /// </summary>
        public void OnStarted() {
            Status = PluginStatus.Started;
        }

        /// <summary>
        /// Called when [finished].
        /// </summary>
        public void OnFinished() {
            if (Status != PluginStatus.Failed) Status = PluginStatus.Finished;
        }

        /// <summary>
        /// Gets the XML Shema.
        /// @warning Leave this always null. See also: https://msdn.microsoft.com/de-de/library/system.xml.serialization.ixmlserializable.getschema%28v=vs.110%29.aspx
        /// </summary>
        /// <returns>Returns null.</returns>
        public System.Xml.Schema.XmlSchema GetSchema() {
            return null;
        }

        /// <summary>
        /// Reads the Node from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        public void ReadXml(System.Xml.XmlReader reader) {
            ReadBasicXmlInfo(reader);

            reader.ReadStartElement();

            XmlSerializer ser = new XmlSerializer(Cache.GetType());
            Cache = ser.Deserialize(reader) as Cache;

            if (!reader.IsStartElement())
                reader.ReadEndElement();
        }

        /// <summary>
        /// Writes the Node to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        public void WriteXml(System.Xml.XmlWriter writer) {
            WriteBasicXmlInfo(writer);

            Cache.Childs = Childs;

            XmlSerializer ser = new XmlSerializer(Cache.GetType());
            ser.Serialize(writer, Cache);
        }

        /// <summary>
        /// Writes basic Informations to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        protected void WriteBasicXmlInfo(System.Xml.XmlWriter writer) {
            writer.WriteAttributeString(nameof(Name), Name);
            writer.WriteAttributeString(nameof(Fullname), Fullname);
            writer.WriteAttributeString(nameof(UID), UID);
        }

        /// <summary>
        /// Reads bais Informations from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        protected void ReadBasicXmlInfo(System.Xml.XmlReader reader) {
            reader.MoveToAttribute(nameof(Name));
            Name = reader.ReadContentAsString();
            reader.MoveToAttribute(nameof(Fullname));
            Fullname = reader.ReadContentAsString();
            reader.MoveToAttribute(nameof(UID));
            UID = reader.ReadContentAsString();
        }
    }
}