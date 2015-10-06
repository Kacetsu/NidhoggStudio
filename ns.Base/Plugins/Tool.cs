using ns.Base.Log;
using ns.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ns.Base.Plugins.Properties;

namespace ns.Base.Plugins {
    /// <summary>
    /// Base Class for all Tools.
    /// </summary>
    [Serializable]
    public class Tool : Plugin, IXmlSerializable  {

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

        public override string Name {
            get {
                if (string.IsNullOrEmpty(_name))
                    _name = this.GetType().Name;
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

            if(_executionTimeMs != null)
                _executionTimeMs.Value = 0;

            foreach (Property childProperty in this.Childs.Where(c => c is Property)) {
                if (childProperty.IsOutput)
                    childProperty.Value = null;
                else if (!string.IsNullOrEmpty(childProperty.ConnectedToUID))
                    childProperty.Value = childProperty.InitialValue;
            }

            return result;
        }

        public override bool PreRun() {
            _timeMeasureStart = DateTime.Now;

            return base.PreRun();
        }

        public override bool PostRun() {
            if(_timeMeasureStart != null)
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

            reader.MoveToAttribute("AssemblyFile");
            AssemblyFile = reader.ReadContentAsString();
            reader.MoveToAttribute("Version");
            Version = reader.ReadContentAsString();

            reader.ReadStartElement();
            if (reader.IsEmptyElement == false) {
                XmlSerializer ser = new XmlSerializer(this.Cache.GetType());
                this.Cache = ser.Deserialize(reader) as Cache;

                List<object> tools = this.Cache.Childs.FindAll(c => c is Tool);
                List<object> propertiesObj = this.Cache.Childs.FindAll(c => c is ns.Base.Plugins.Properties.Property);

                List<Property> properties = new List<Property>();
                foreach (Property p in propertiesObj)
                    properties.Add(p);

                if (tools != null)
                    this.Childs = new List<object>(tools);

                this.Childs.AddRange(properties);
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
        /// Writes the Tool to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
            WriteBasicXmlInfo(writer);
            writer.WriteAttributeString("AssemblyFile", AssemblyFile);
            writer.WriteAttributeString("Version", Version);

            this.Cache.Childs = new List<object>(this.Childs);

            try {
                XmlSerializer ser = new XmlSerializer(this.Cache.GetType());
                ser.Serialize(writer, this.Cache);
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }
    }
}
