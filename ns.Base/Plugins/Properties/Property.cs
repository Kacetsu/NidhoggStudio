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
        private object _initialValue = null;
        private string _groupName = string.Empty;
        private bool _isMonitored = false;
        private bool _canAutoConnect = false;
        private bool _isToleranceDisabled = true;
        private Tolerance<object> _tolerance;
        

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName {
            get {
                string result = _name;
                if (!string.IsNullOrEmpty(_groupName))
                    result = _groupName;
                return result;
            }
            set { 
                _groupName = value;
                OnPropertyChanged("GroupName");
            }
        }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public object Value {
            get { return _value; }
            set { 
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets the initial value.
        /// </summary>
        /// <value>
        /// The initial value.
        /// </value>
        public object InitialValue {
            get { return _initialValue; }
        }

        /// <summary>
        /// Gets or sets the unified identification of the parent tool.
        /// </summary>
        [XmlAttribute("ToolUID")]
        public string ToolUID {
            get { return _toolUid; }
            set { 
                _toolUid = value;
                OnPropertyChanged("ToolUID");
            }
        }

        /// <summary>
        /// Gets or sets if the property is used as output.
        /// </summary>
        [XmlAttribute("IsOutput")]
        public bool IsOutput {
            get { return _isOutput; }
            set { 
                _isOutput = value;
                OnPropertyChanged("IsOutput");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is monitored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is monitored; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute("IsMonitored")]
        public bool IsMonitored {
            get { return _isMonitored; }
            set { 
                _isMonitored = value;
                OnPropertyChanged("IsMonitored");
            }
        }

        /// <summary>
        /// Gets or sets the UID of the property that is connected to this one.
        /// </summary>
        [XmlAttribute("ConnectedToUID")]
        public string ConnectedToUID {
            get { return _connectedToUid; }
            set { 
                _connectedToUid = value;
                OnPropertyChanged("ConnectedToUID");
            }
        }

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
                OnPropertyChanged("CanAutoConnect");
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        [XmlAttribute("Type")]
        public virtual Type Type {
            get { return typeof(object); }
        }

        public virtual bool IsToleranceDisabled {
            get { return _isToleranceDisabled; }
            set {
                _isToleranceDisabled = value;
                OnPropertyChanged("IsToleranceDisabled");
            }
        }

        public virtual Tolerance<object> Tolerance {
            get { return _tolerance; }
            set { _tolerance = value; }
        }

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        public Property() {
            this.Name = "UNKNOWN";
        }

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        public Property(string name, object value) {
            this.Name = name;
            this.Value = value;
            this.UID = Property.GenerateUID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="value">The value.</param>
        public Property(string name, string groupName, object value) {
            this.Name = name;
            this.GroupName = groupName;
            this.Value = value;
            this.UID = Property.GenerateUID();
        }

        /// <summary>
        /// Base Class for all Properties.
        /// @warning Should not be used directly.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public Property(string name, bool isOutput) {
            this.Name = name;
            this.Value = null;
            this.UID = Property.GenerateUID();
            this.IsOutput = isOutput;
        }

        public Property(Property property) : base(property) {
            this.Value = property.Value;
        }

        /// <summary>
        /// Connects the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Connect(Property property) {
            if (_connectedProperty == property) return;

            _initialValue = this.Value;
            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            _connectedProperty = property;
            this.ConnectedToUID = _connectedProperty.UID;
            _connectedProperty.PropertyChanged += ConnectedPropertyChangedHandle;
            _connectedProperty.PropertyUnconnectEvent += ConnectedPropertyUnconnectEvent;
        }

        /// <summary>
        /// Connects the specified uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void Connect(string uid) {
            this.ConnectedToUID = uid;
        }

        /// <summary>
        /// Unconnects this instance.
        /// </summary>
        public void Unconnect() {
            this.ConnectedToUID = string.Empty;

            if (this.PropertyUnconnectEvent != null)
                this.PropertyUnconnectEvent(this, new NodeChangedEventArgs(this));

            if (_connectedProperty != null) {
                _connectedProperty.PropertyChanged -= ConnectedPropertyChangedHandle;
                _connectedProperty.PropertyUnconnectEvent -= ConnectedPropertyUnconnectEvent;
            }
            this.Value = _initialValue;
            _connectedProperty = null;
        }

        /// <summary>
        /// Parents the property changed handle.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ConnectedPropertyChangedHandle(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Value") {
                if (_initialValue == null) _initialValue = _value;
                _value = _connectedProperty.Value;
            }
        }

        /// <summary>
        /// Connecteds the property unconnect event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ConnectedPropertyUnconnectEvent(object sender, NodeChangedEventArgs e) {
            this.Unconnect();
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Property clone = base.Clone() as Property;
            clone.Value = this.Value;
            return clone;
        }

        public virtual void Save(XmlWriter writer) {
            WriteXml(writer);
        }

        public virtual void Load(XmlReader reader) {
            ReadXml(reader);
        }

        /// <summary>
        /// Gets the XML Shema.
        /// @warning Leave this always null. See also: https://msdn.microsoft.com/de-de/library/system.xml.serialization.ixmlserializable.getschema%28v=vs.110%29.aspx
        /// </summary>
        /// <returns>Returns null.</returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
            return null;
        }

        /// <summary>
        /// Reads the Property from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
            ReadBasicXmlInfo(reader);
            if(reader.MoveToAttribute("IsOutput"))
                this.IsOutput = Convert.ToBoolean(reader.ReadContentAsString());
            if (reader.MoveToAttribute("IsMonitored"))
                this.IsMonitored = Convert.ToBoolean(reader.ReadContentAsString());
            if(reader.MoveToAttribute("ConnectedToUID"))
                this.ConnectedToUID = reader.ReadContentAsString();

            if (reader.IsEmptyElement == false) {
                reader.ReadStartElement();
                if (reader.Name == "anyType") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(object));
                        this.Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                } else if(reader.Name == "ArrayOfDouble") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(List<double>));
                        this.Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                } else if (reader.Name == "ArrayOfInteger") {
                    try {
                        XmlSerializer ser = new XmlSerializer(typeof(List<int>));
                        this.Value = ser.Deserialize(reader);
                    } catch (Exception ex) {
                        Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                    }
                }
            }

            if (reader.Name == "Cache" && reader.IsStartElement()) {
                reader.ReadStartElement();
                try {
                    XmlSerializer ser = new XmlSerializer(this.Cache.GetType());
                    this.Cache = ser.Deserialize(reader) as Cache;
                } catch (Exception ex) {
                    Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                }
                this.Childs = new List<object>(this.Cache.Childs);
            }

            if(reader.Name.Contains("ToleranceOf") && reader.IsStartElement()) {
                reader.ReadStartElement();
                this.Tolerance = new Tolerance<object>();
                string min = reader.ReadInnerXml();
                string max = reader.ReadInnerXml();
                this.Tolerance.Min = min;
                this.Tolerance.Max = max;
            }

            while (!reader.IsStartElement()) {
                if (reader.Name == "Cache") {
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
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
            WriteBasicXmlInfo(writer);
            if(this.IsOutput)
                writer.WriteAttributeString("IsOutput", this.IsOutput.ToString());
            if (this.IsMonitored)
                writer.WriteAttributeString("IsMonitored", this.IsMonitored.ToString());
            if(!string.IsNullOrEmpty(this.ConnectedToUID))
                writer.WriteAttributeString("ConnectedToUID", this.ConnectedToUID);

            this.Cache.Childs = new List<object>();

            foreach (Node child in this.Childs) {
                if (child is Property)
                    this.Cache.Childs.Add(child);
            }

            if (this.Value != null && !this.IsOutput) {
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(object));
                    if (this is DeviceProperty)
                        ser.Serialize(writer, ((DeviceProperty)this).DeviceUID);
                    else if (this is ListProperty)
                        ser.Serialize(writer, this.Value.ToString());
                    else if (this is RectangleProperty) {
                        ser = new XmlSerializer(typeof(List<double>));
                        ser.Serialize(writer, this.Value);
                    } else
                        ser.Serialize(writer, this.Value);
                    if (this.Cache.Childs.Count > 0) {
                        ser = new XmlSerializer(this.Cache.GetType());
                        ser.Serialize(writer, this.Cache);
                    }
                } catch (Exception ex) {
                    Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
                }
            }

            if(!this.IsToleranceDisabled) {
                this.Save(writer);
            }

        }
    }
}
