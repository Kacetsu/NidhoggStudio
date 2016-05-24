using ns.Base.Log;
using ns.Base.Plugins.Properties;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace ns.Base.Plugins {
    /// <summary>
    /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
    /// </summary>
    [Serializable]
    public class Plugin : Node, IPlugin, ICloneable {

        private string _version = string.Empty;
        private string _assemblyFile = string.Empty;
        private string _displayName = string.Empty;
        private PluginStatus _status = PluginStatus.Unknown;

        /// <summary>
        /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
        /// Creates additional to the BaseNode the following fields: AssemblyFile and Version.
        /// </summary>
        public Plugin() : base() { }

        /// <summary>
        /// Gets or sets the AssemblyFile. 
        /// Used to find the correct Plugin while loading the CoreSystem / PluginManager.
        /// </summary>
        [XmlAttribute("AssemblyFile")]
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
        [XmlAttribute("Version")]
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
                    OnPropertyChanged("DisplayName");
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

        public PluginStatus Status {
            get { return _status; }
            set {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

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
            lock (this.Childs) {
                foreach (Plugin child in this.Childs.Where(p => p is Plugin)) {
                    if (child.PreRun() == false) {
                        Trace.WriteLine("Plugin " + child.Name + " pre run failed!", LogCategory.Error);
                        result = false;
                    }else if (child.Run() == false) {
                        Trace.WriteLine("Plugin " + child.Name + " run failed!", LogCategory.Error);
                        result = false;
                    }else if (child.PostRun() == false) {
                        Trace.WriteLine("Plugin " + child.Name + " post run failed!", LogCategory.Error);
                        result = false;
                    }

                    foreach (Property property in child.Childs) {
                        if (property.IsToleranceDisabled) continue;
                        if (property is DoubleProperty) {
                            DoubleProperty targetProperty = property as DoubleProperty;
                            double min = targetProperty.Tolerance.Min;
                            double max = targetProperty.Tolerance.Max;
                            double value = Convert.ToDouble(targetProperty.Value);
                            if (value > max || value < min) {
                                result = false;
                            }
                        } else if (property is IntegerProperty) {
                            IntegerProperty targetProperty = property as IntegerProperty;
                            int min = targetProperty.Tolerance.Min;
                            int max = targetProperty.Tolerance.Max;
                            int value = Convert.ToInt32(targetProperty.Value);
                            if (value > max || value < min) {
                                result = false;
                            }
                        }
                    }

                    if (!result) break;
                }

                if(!result) {
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

            foreach (Property property in this.Childs.Where(p => p is Property)) {
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

            foreach (Property property in this.Childs.Where(p => p is Property)) {
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
            this.Status = PluginStatus.Started;
        }

        /// <summary>
        /// Called when [finished].
        /// </summary>
        public void OnFinished() {
            if(this.Status != PluginStatus.Failed) this.Status = PluginStatus.Finished;
        }
    }
}
