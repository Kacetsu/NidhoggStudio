using ns.Base.Extensions;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base.Plugins {

    [DataContract]
    public class Device : Plugin {

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Device clone = this.DeepClone();
            clone.UID = GenerateUID();
            return clone;
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>
        /// Success of the Operation.
        /// </returns>
        public override bool Finalize() {
            bool result = base.Finalize();

            foreach (Property childProperty in Childs.Where(c => c is Property)) {
                IValue<object> valueProperty = childProperty as IValue<object>;
                if (valueProperty == null) continue;

                if (childProperty.IsOutput)
                    valueProperty.Value = null;
                else if (!string.IsNullOrEmpty(childProperty.ConnectedUID))
                    valueProperty.Value = (childProperty as IConnectable<object>)?.InitialValue;
            }

            return result;
        }

        ///// <summary>
        ///// Gets the XML Shema.
        ///// @warning Leave this always null. See also: https://msdn.microsoft.com/de-de/library/system.xml.serialization.ixmlserializable.getschema%28v=vs.110%29.aspx
        ///// </summary>
        ///// <returns>
        ///// Returns null.
        ///// </returns>
        //System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
        //    return null;
        //}

        ///// <summary>
        ///// Reads the Node from the XML.
        ///// </summary>
        ///// <param name="reader">The instance of the XmlReader.</param>
        //void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) {
        //    ReadBasicXmlInfo(reader);

        //    reader.MoveToAttribute("AssemblyFile");
        //    AssemblyFile = reader.ReadContentAsString();
        //    reader.MoveToAttribute("Version");
        //    Version = reader.ReadContentAsString();

        //    reader.ReadStartElement();
        //    if (reader.IsEmptyElement == false) {
        //        XmlSerializer ser = new XmlSerializer(Cache.GetType());
        //        Cache = ser.Deserialize(reader) as Cache;

        //        IEnumerable<Node> propertiesObj = Cache.Childs.Where(c => c is Property);

        //        foreach (Property p in propertiesObj) Childs.Add(p);
        //    }
        //}

        ///// <summary>
        ///// Writes the Node to the XML.
        ///// </summary>
        ///// <param name="writer">The instance of the XmlWriter.</param>
        //void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer) {
        //    WriteBasicXmlInfo(writer);
        //    writer.WriteAttributeString("AssemblyFile", AssemblyFile);
        //    writer.WriteAttributeString("Version", Version);

        //    Cache.Childs = new ObservableList<Node>(Childs);

        //    List<Property> filteredProperties = new List<Property>();

        //    foreach (Property property in Cache.Childs.Where(c => c is Property)) {
        //        if (property.IsOutput) {
        //            filteredProperties.Add(property);
        //        }
        //    }

        //    foreach (Property property in filteredProperties) {
        //        Cache.Childs.Remove(property);
        //    }

        //    try {
        //        XmlSerializer ser = new XmlSerializer(Cache.GetType());
        //        ser.Serialize(writer, Cache);
        //    } catch (Exception ex) {
        //        Log.Trace.WriteLine(ex.Message, ex.StackTrace, TraceEventType.Error);
        //    }
        //}
    }
}