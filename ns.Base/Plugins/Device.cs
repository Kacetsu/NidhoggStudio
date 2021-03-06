﻿using ns.Base.Extensions;
using ns.Base.Log;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ns.Base.Plugins
{
    [Serializable]
    public class Device : Plugin, IXmlSerializable {
        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>
        /// The cloned Node.
        /// </returns>
        public override object Clone() {
            Device clone = this.DeepClone();
            clone.UID = Node.GenerateUID();
            return clone;
        }

        public override bool Finalize() {
            bool result = base.Finalize();

            foreach (Property childProperty in this.Childs.Where(c => c is Property)) {
                if (childProperty.IsOutput)
                    childProperty.Value = null;
                else if (!string.IsNullOrEmpty(childProperty.ConnectedToUID))
                    childProperty.Value = childProperty.InitialValue;
            }

            return result;
        }

        /// <summary>
        /// Gets the XML Shema.
        /// @warning Leave this always null. See also: https://msdn.microsoft.com/de-de/library/system.xml.serialization.ixmlserializable.getschema%28v=vs.110%29.aspx
        /// </summary>
        /// <returns>
        /// Returns null.
        /// </returns>
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() {
            return null;
        }

        /// <summary>
        /// Reads the Node from the XML.
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

                List<object> propertiesObj = this.Cache.Childs.FindAll(c => c is Property);

                List<Property> properties = new List<Property>();
                foreach (Property p in propertiesObj)
                    properties.Add(p);

                this.Childs.AddRange(properties);
            }
        }

        /// <summary>
        /// Writes the Node to the XML.
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
