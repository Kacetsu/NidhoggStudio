using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace ns.Base.Plugins {

    /// <summary>
    /// Base Class for all Tools.
    /// </summary>
    [Serializable]
    public class Tool : Plugin, IXmlSerializable {
        private DateTime _timeMeasureStart;
        private DoubleProperty _executionTimeMs;

        /// <summary>
        /// Base Class for all Tools.
        /// Creates the field: Properties.
        /// </summary>
        public Tool() : base() {
            DoubleProperty executionTimeMs = new DoubleProperty("ExecutionTimeMs", true);
            executionTimeMs.Tolerance = new Tolerance<double>(0, 1000);
            AddChild(executionTimeMs);
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public override string Name {
            get {
                if (string.IsNullOrEmpty(_name)) {
                    if (!string.IsNullOrEmpty(DisplayName))
                        _name = DisplayName;
                    else
                        _name = this.GetType().Name;
                }
                return _name;
            }
            set {
                base.Name = value;
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public virtual string Category {
            get { return string.Empty; }
        }

        /// <summary>
        /// Clones the Tools as deep Clone.
        /// Generates a new UID for the clones Tool.
        /// </summary>
        /// <returns>Returns the cloned Tool.</returns>
        public override object Clone() {
            Tool clone = this.DeepClone();
            clone.UID = Node.GenerateUID();
            return clone;
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Initialize() {
            _executionTimeMs = GetProperty("ExecutionTimeMs") as DoubleProperty;
            return base.Initialize();
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            bool result = base.Finalize();

            if (_executionTimeMs != null)
                _executionTimeMs.Value = 0;

            foreach (Property childProperty in this.Childs.Where(c => c is Property)) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.IsOutput) {
                    valueProperty.Value = null;
                } else if (!string.IsNullOrEmpty(childProperty.ConnectedUID)) {
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        public override bool PreRun() {
            _timeMeasureStart = DateTime.Now;

            return base.PreRun();
        }

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        public override bool PostRun() {
            if (_timeMeasureStart != null)
                _executionTimeMs.Value = DateTime.Now.Subtract(_timeMeasureStart).TotalMilliseconds;

            return base.PostRun();
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
        /// Reads the Tool from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
            ReadBasicXmlInfo(reader);

            reader.MoveToAttribute(nameof(AssemblyFile));
            AssemblyFile = reader.ReadContentAsString();
            reader.MoveToAttribute(nameof(Version));
            Version = reader.ReadContentAsString();

            reader.ReadStartElement();
            if (reader.IsEmptyElement == false) {
                XmlSerializer ser = new XmlSerializer(Cache.GetType());
                Cache = ser.Deserialize(reader) as Cache;

                IEnumerable<Node> tools = Cache.Childs.Where(c => c is Tool);
                IEnumerable<Node> propertiesObj = Cache.Childs.Where(c => c is Property);

                if (tools != null)
                    Childs = new ObservableList<Node>(tools);

                List<Property> properties = new List<Property>();
                foreach (Property p in propertiesObj) Childs.Add(p);
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
        /// Writes the Tool to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
            WriteBasicXmlInfo(writer);
            writer.WriteAttributeString(nameof(AssemblyFile), AssemblyFile);
            writer.WriteAttributeString(nameof(Version), Version);

            Cache.Childs = new ObservableList<Node>(Childs);

            try {
                XmlSerializer ser = new XmlSerializer(Cache.GetType());
                ser.Serialize(writer, Cache);
            } catch (Exception ex) {
                Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
            }
        }
    }
}