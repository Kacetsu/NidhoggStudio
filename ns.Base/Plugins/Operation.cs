using ns.Base.Event;
using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ns.Base.Plugins.Properties;

namespace ns.Base.Plugins {
    /// <summary>
    /// Base Class for all Operations.
    /// Instead of all other Base Classes this one can be used directly as a functional Operation.
    /// </summary>
    [Serializable]
    public class Operation : Plugin, IXmlSerializable {

        private string _linkedOperation = string.Empty;

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        public Operation() : base() {
            DisplayName = "Operation";
            AddChild(new OperationSelectionProperty("LinkedOperation", false));
            AddChild(new ListProperty("Trigger", Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList()));
        }

        /// <summary>
        /// Base Class for all Operations.
        /// Instead of all other Base Classes this one can be used directly as a functional Operation.
        /// </summary>
        /// <param name="name">The name of the Operation.</param>
        public Operation(string name) : base() {
            this.Name = name;
            AddChild(new OperationSelectionProperty("LinkedOperation", false));
            AddChild(new ListProperty("Trigger", Enum.GetValues(typeof(OperationTrigger)).Cast<object>().ToList()));
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Operation clone = this.DeepClone();
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
            base.Initialize();

            foreach (Tool tool in this.Childs.Where(t => t is Tool)) {
                if (!tool.Initialize())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Finalize the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            return base.Finalize(); ;
        }

        /// <summary>
        /// Run the Plugin.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Run() {
            return RunChilds();
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
        /// Reads the Operation from the XML.
        /// </summary>
        /// <param name="reader">The instance of the XmlReader.</param>
        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
            ReadBasicXmlInfo(reader);

            reader.ReadStartElement();

            XmlSerializer ser = new XmlSerializer(this.Cache.GetType());
            this.Cache = ser.Deserialize(reader) as Cache;

            this.Childs = new List<object>(this.Cache.Childs);
        }

        /// <summary>
        /// Writes the Operation to the XML.
        /// </summary>
        /// <param name="writer">The instance of the XmlWriter.</param>
        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
            this.Cache.Childs = this.Childs;

            WriteBasicXmlInfo(writer);

            try {
                XmlSerializer ser = new XmlSerializer(this.Cache.GetType());
                ser.Serialize(writer, this.Cache);
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }
        }
    }
}
