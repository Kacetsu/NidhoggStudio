using ns.Base.Event;
using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    /// <summary>
    /// Base Class for all Properties.
    /// @warning Should not be used directly.
    /// </summary>
    [DataContract
        KnownType(typeof(DeviceProperty)),
        KnownType(typeof(ImageProperty)),
        KnownType(typeof(ListProperty)),
        KnownType(typeof(NumberProperty<object>)),
        KnownType(typeof(RectangleProperty)),
        KnownType(typeof(StringProperty))]
    public abstract class Property : Node {
        private bool _canAutoConnect = false;

        private Property _connectedProperty = null;

        private string _connectedUid = string.Empty;

        private string _groupName = string.Empty;

        private bool _isMonitored = false;

        private bool _isOutput = false;

        private bool _isToleranceDisabled = true;

        private string _toolUid = string.Empty;

        private string _uid = string.Empty;

        private object _value = null;

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        public Property() {
            Name = "UNKNOWN";
            UID = GenerateUID();
        }

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        public Property(string name) : this() {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public Property(string name, string groupName) : this(name) {
            GroupName = groupName;
        }

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public Property(string name, bool isOutput) : this() {
            Name = name;
            IsOutput = isOutput;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public Property(Property property) : base(property) {
            IsOutput = property.IsOutput;
            GroupName = property.GroupName;
            IsMonitored = property.IsMonitored;
        }

        /// <summary>
        /// Occurs when [property unconnect event].
        /// </summary>
        protected event EventHandler<NodeChangedEventArgs> PropertyUnconnectEvent;

        /// <summary>
        /// Gets or sets a value indicating whether this instance can automatic connect.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can automatic connect; otherwise, <c>false</c>.
        /// </value>
        public bool CanAutoConnect {
            get { return _canAutoConnect; }
            set {
                _canAutoConnect = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the connected property.
        /// </summary>
        /// <value>
        /// The connected property.
        /// </value>
        public Property ConnectedProperty => _connectedProperty;

        /// <summary>
        /// Gets or sets the UID of the property that is connected to this one.
        /// </summary>
        [DataMember]
        public string ConnectedUID {
            get { return _connectedUid; }
            set {
                _connectedUid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName {
            get {
                string result = Name;
                if (!string.IsNullOrEmpty(_groupName))
                    result = _groupName;
                return result;
            }
            set {
                _groupName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is monitored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is monitored; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsMonitored {
            get { return _isMonitored; }
            set {
                _isMonitored = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the property is used as output.
        /// </summary>
        [DataMember]
        public bool IsOutput {
            get { return _isOutput; }
            set {
                _isOutput = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the unified identification of the parent tool.
        /// </summary>
        [DataMember]
        public string ToolUID {
            get { return _toolUid; }
            set {
                _toolUid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public virtual Type Type => typeof(object);

        /// <summary>
        /// Connects the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public virtual void Connect(Property property) {
            if (_connectedProperty == property) return;

            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            _connectedProperty = property;
            ConnectedUID = _connectedProperty.UID;
            _connectedProperty.PropertyChanged += ConnectedPropertyChangedHandle;
            _connectedProperty.PropertyUnconnectEvent += ConnectedPropertyUnconnectEvent;
        }

        /// <summary>
        /// Connects the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Connect(string uid) => ConnectedUID = uid;

        /// <summary>
        /// Unconnects this instance.
        /// </summary>
        public virtual void Unconnect() {
            ConnectedUID = string.Empty;

            PropertyUnconnectEvent?.Invoke(this, new NodeChangedEventArgs(this));

            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            _connectedProperty = null;
        }

        /// <summary>
        /// Parents the property changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void ConnectedPropertyChangedHandle(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        }

        /// <summary>
        /// Connecteds the property unconnect event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ConnectedPropertyUnconnectEvent(object sender, NodeChangedEventArgs e) => Unconnect();
    }
}