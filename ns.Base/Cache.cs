using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
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
        XmlElement(typeof(ListProperty)),
        XmlElement(typeof(Property))]
        public List<object> Childs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> class.
        /// </summary>
        public Cache() {
            Childs = new List<object>();
        }

    }
}
