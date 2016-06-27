using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
    /// </summary>
    [DataContract(IsReference = true), KnownType(typeof(Operation)), KnownType(typeof(Tool)), KnownType(typeof(Device))]
    public class Plugin : Node, IPlugin, ICloneable {
        private string _version = string.Empty;

        private string _assemblyFile = string.Empty;

        private string _displayName = string.Empty;

        private PluginStatus _status = PluginStatus.Unknown;

        /// <summary>
        /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
        /// Creates additional to the BaseNode the following fields: AssemblyFile and Version.
        /// </summary>
        public Plugin() : base() {
        }

        public Plugin(Plugin other) : base(other) {
            DisplayName = other.DisplayName;
            Status = other.Status;
        }

        /// <summary>
        /// Gets the assembly file.
        /// </summary>
        /// <value>
        /// The assembly file.
        /// </value>
        public string AssemblyFile {
            get {
                if (string.IsNullOrEmpty(_assemblyFile)) {
                    Assembly assembly = Assembly.GetAssembly(GetType());
                    _assemblyFile = Path.GetFileNameWithoutExtension(assembly.Location);
                }
                return _assemblyFile;
            }
        }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        [DataMember]
        public string Version {
            get {
                if (string.IsNullOrEmpty(_version)) {
                    Assembly assembly = Assembly.GetAssembly(GetType());
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
        [DataMember]
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(_displayName))
                    _displayName = GetType().Name;
                return _displayName;
            }
            set {
                if (_displayName == null || !_displayName.Equals(value)) {
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
        [DataMember]
        public PluginStatus Status {
            get { return _status; }
            set {
                _status = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public virtual bool PreRun() {
            OnStarted();
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
            OnFinished();
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
    }
}