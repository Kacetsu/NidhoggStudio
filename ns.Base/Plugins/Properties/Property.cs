using ns.Base.Event;
using ns.Base.Log;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ns.Base.Plugins.Properties {

    /// <summary>
    /// Base Class for all Properties.
    /// @warning Should not be used directly.
    /// </summary>
    [Serializable]
    public class Property : Node, IXmlSerializable {

        public event NodeChangedEventHandler PropertyUnconnectEvent;

        private object _value = null;
        private string _uid = string.Empty;
        private string _toolUid = string.Empty;
        private bool _isOutput = false;
        private string _connectedToUid = string.Empty;
        private Property _connectedProperty = null;
        private string _groupName = string.Empty;
        private bool _isMonitored = false;
        private bool _canAutoConnect = false;
        private bool _isToleranceDisabled = true;

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        [XmlIgnore]
        public string GroupName {
            get {
                string result = _name;
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
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value {
            get { return _value; }
            set {
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        [XmlIgnore]
        public object InitialValue { get; private set; } = null;

        /// <summary>
        /// Gets or sets the unified identification of the parent tool.
        /// </summary>
        [XmlAttribute]
        public string ToolUID {
            get { return _toolUid; }
            set {
                _toolUid = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets if the property is used as output.
        /// </summary>
        [XmlAttribute]
        public bool IsOutput {
            get { return _isOutput; }
            set {
                _isOutput = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is monitored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is monitored; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool IsMonitored {
            get { return _isMonitored; }
            set {
                _isMonitored = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets if the Property has a numeric value.
        /// </summary>
        public virtual bool IsNumeric => false;

        /// <summary>
        /// Gets or sets the UID of the property that is connected to this one.
        /// </summary>
        [XmlAttribute]
        public string ConnectedToUID {
            get { return _connectedToUid; }
            set {
                _connectedToUid = value;
                OnPropertyChanged();
            }
        }

        public Property ConnectedProperty => null;

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
        /// Gets the type of the property.
        /// </summary>
        [XmlIgnore]
        public virtual Type Type => typeof(object);

        public virtual bool IsToleranceDisabled {
            get { return _isToleranceDisabled; }
            set {
                _isToleranceDisabled = value;
                OnPropertyChanged();
            }
        }

        public virtual Tolerance<object> Tolerance { get; set; }

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
        public Property(string name, object value) : this() {
            Name = name;
            Value = value;
            InitialValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public Property(string name, string groupName, object value) : this(name, value) {
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
            Value = null;
            IsOutput = isOutput;
        }

        public Property(Property property) : base(property) {
            Value = property.Value;
            InitialValue = property.InitialValue;
            IsOutput = property.IsOutput;
            GroupName = property.GroupName;
            IsMonitored = property.IsMonitored;
        }

        /// <summary>
        /// Connects the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Connect(Property property) {
            if (_connectedProperty == property) return;

            InitialValue = Value;
            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            _connectedProperty = property;
            ConnectedToUID = _connectedProperty.UID;
            _connectedProperty.PropertyChanged += ConnectedPropertyChangedHandle;
            _connectedProperty.PropertyUnconnectEvent += ConnectedPropertyUnconnectEvent;
        }

        /// <summary>
        /// Connects the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Connect(string uid) => ConnectedToUID = uid;

        /// <summary>
        /// Unconnects this instance.
        /// </summary>
        public void Unconnect() {
            ConnectedToUID = string.Empty;

            PropertyUnconnectEvent?.Invoke(this, new NodeChangedEventArgs(this));

            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            Value = InitialValue;
            _connectedProperty = null;
        }

        /// <summary>
        /// Parents the property changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ConnectedPropertyChangedHandle(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Value)) {
                if (InitialValue == null) InitialValue = _value;
                _value = _connectedProperty.Value;
            }
        }

        /// <summary>
        /// Connecteds the property unconnect event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ConnectedPropertyUnconnectEvent(object sender, NodeChangedEventArgs e) => Unconnect();

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Property clone = base.Clone() as Property;
            clone.Value = Value;
            return clone;
        }

        public virtual void Save(XmlWriter writer) => WriteXml(writer);

        public virtual void Load(XmlReader reader) => ReadXml(reader);

        /// <summary>
        /// Gets the XML Shema.
        /// @warning Leave this always null. See also: https://msdn.microsoft.com/de-de/library/system.xml.serialization.ixmlserializable.getschema%28v=vs.110%29.aspx
        /// </summary>
        /// <returns>Returns null.</returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() => null;

        /// <summary>
        /// Reads the Property from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        void IXmlSerializable.ReadXml(XmlReader reader) {
            ReadBasicXmlInfo(reader);
            if (reader.MoveToAttribute(nameof(IsOutput))) IsOutput = Convert.ToBoolean(reader.ReadContentAsString());

            if (reader.MoveToAttribute(nameof(IsMonitored))) IsMonitored = Convert.ToBoolean(reader.ReadContentAsString());

            if (reader.MoveToAttribute(nameof(ConnectedToUID))) ConnectedToUID = reader.ReadContentAsString();

            if (!reader.IsEmptyElement) {
                reader.ReadStartElement();
                if (reader.Name == "anyType") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(object));
                        Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                } else if (reader.Name == "ArrayOfDouble") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(List<double>));
                        Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                } else if (reader.Name == "ArrayOfInteger") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(List<int>));
                        Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                }
            }

            if (reader.Name == nameof(Cache) && reader.IsStartElement()) {
                reader.ReadStartElement();
                try {
                    XmlSerializer ser = new XmlSerializer(Cache.GetType());
                    Cache = ser.Deserialize(reader) as Cache;
                } catch (Exception ex) {
                    Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                }
                Childs = new ObservableList<object>(Cache.Childs);
            }

            if (reader.Name.Contains("ToleranceOf") && reader.IsStartElement()) {
                reader.ReadStartElement();
                Tolerance = new Tolerance<object>();
                string min = reader.ReadInnerXml();
                string max = reader.ReadInnerXml();
                Tolerance.Min = min;
                Tolerance.Max = max;
            }

            while (!reader.IsStartElement()) {
                if (reader.Name == nameof(Cache)) {
                    reader.ReadEndElement();
                    break;
                }

                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Writes the Property to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer) {
            WriteBasicXmlInfo(writer);
            if (IsOutput) writer.WriteAttributeString(nameof(IsOutput), IsOutput.ToString());

            if (IsMonitored) writer.WriteAttributeString(nameof(IsMonitored), IsMonitored.ToString());

            if (!string.IsNullOrEmpty(ConnectedToUID)) writer.WriteAttributeString(nameof(ConnectedToUID), ConnectedToUID);

            Cache.Childs = new ObservableList<object>();

            foreach (Node child in Childs) {
                if (child is Property)
                    Cache.Childs.Add(child);
            }

            if (Value != null && !IsOutput) {
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(object));
                    if (this is DeviceProperty)
                        ser.Serialize(writer, ((DeviceProperty)this).DeviceUID);
                    else if (this is ListProperty)
                        ser.Serialize(writer, Value.ToString());
                    else if (this is RectangleProperty) {
                        ser = new XmlSerializer(typeof(List<double>));
                        ser.Serialize(writer, Value);
                    } else
                        ser.Serialize(writer, Value);
                    if (Cache.Childs.Count > 0) {
                        ser = new XmlSerializer(Cache.GetType());
                        ser.Serialize(writer, Cache);
                    }
                } catch (Exception ex) {
                    Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                }
            }

            if (!IsToleranceDisabled) {
                Save(writer);
            }
        }
    }
}