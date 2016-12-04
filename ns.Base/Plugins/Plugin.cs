using ns.Base.Plugins.Devices;
using ns.Base.Plugins.Properties;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class used for all Plugins (Tools, Devices, Extensions, Operations).
    /// Creates additional to the BaseNode the following fields: AssemblyFile and Version.
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(Operation))]
    [KnownType(typeof(Tool))]
    [KnownType(typeof(Device))]
    [KnownType(typeof(Factory))]
    public class Plugin : Node, IPlugin {
        private string _assemblyFile = string.Empty;
        private string _displayName = string.Empty;
        private PluginStatus _status = PluginStatus.Unknown;
        private string _version = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
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
        /// Gets or sets the Description.
        /// The Description is used for the Application User to visualize a human readable Name.
        /// </summary>
        public virtual string Description => string.Empty;

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
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override Node Clone() => new Plugin(this);

        /// <summary>
        /// Finds the or add.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TType FindOrAdd<TType, TValue>(TValue value, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null) where TType
            : GenericProperty<TValue> {
            TType node = Items.FirstOrDefault(i => string.Equals(i.Value.Name, name, StringComparison.Ordinal)).Value as TType;
            if (node == null) {
                node = Activator.CreateInstance(typeof(TType), value, direction, name) as TType;
                Items.TryAdd(node.Id, node);
            }

            return node;
        }

        /// <summary>
        /// Finds the or add.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TType FindOrAdd<TType, TValue>(TValue value, TValue min, TValue max, PropertyDirection direction = PropertyDirection.In, [CallerMemberName] string name = null) where TType
            : GenericProperty<TValue> {
            TType node = Items.FirstOrDefault(i => string.Equals(i.Value.Name, name, StringComparison.Ordinal)).Value as TType;
            if (node == null) {
                node = Activator.CreateInstance(typeof(TType), value, min, max, direction, name) as TType;
                Items.TryAdd(node.Id, node);
            }

            return node;
        }

        /// <summary>
        /// Called when [finished].
        /// </summary>
        public void OnFinished() {
            if (Status != PluginStatus.Failed) Status = PluginStatus.Finished;
        }

        /// <summary>
        /// Called when [started].
        /// </summary>
        public void OnStarted() {
            Status = PluginStatus.Started;
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Terminate() => true;

        /// <summary>
        /// Tries the get property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public bool TryGetProperty<T>(out T property) where T : Property {
            property = (T)Items.Values.FirstOrDefault(p => p is T);
            return property != null;
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryPostRun() {
            OnFinished();
            return true;
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryPreRun() {
            OnStarted();
            return true;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>Success of the Operation.</returns>
        public virtual bool TryRun() => true;

        /// <summary>
        /// Runs the childs.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryRunChilds() {
            bool result = true;
            lock (Items) {
                foreach (Tool child in Items.Values.OfType<Tool>()) {
                    if (child.TryPreRun() == false) {
                        Log.Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Tool {0} pre run failed!", child.Name), TraceEventType.Error);
                        result = false;
                    } else if (child.TryRun() == false) {
                        Log.Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Tool {0} run failed!", child.Name), TraceEventType.Error);
                        result = false;
                    } else if (child.TryPostRun() == false) {
                        Log.Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Tool {0} post run failed!", child.Name), TraceEventType.Error);
                        result = false;
                    }

                    foreach (Property property in child.Items.Values) {
                        ITolerance tolerancProperty = property as ITolerance;
                        if (tolerancProperty == null || tolerancProperty.IsToleranceEnabled == false) continue;

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
    }
}