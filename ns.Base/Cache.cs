using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Xml.Serialization;

namespace ns.Base {

    [Serializable]
    public class Cache {

        /// <summary>
        /// Gets or sets the childs.
        /// </summary>
        /// <value>
        /// The childs.
        /// </value>
        [XmlElement(typeof(Operation)),
            XmlElement(typeof(Tool)),
            XmlElement(typeof(Extension)),
            XmlElement(typeof(Property))]
        public ObservableList<Node> Childs { get; set; } = new ObservableList<Node>();
    }
}