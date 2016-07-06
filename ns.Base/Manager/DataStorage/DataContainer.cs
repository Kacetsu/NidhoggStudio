using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Collections.Generic;

namespace ns.Base.Manager.DataStorage {

    public abstract class DataContainer : Node {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContainer"/> class.
        /// </summary>
        public DataContainer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContainer"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public DataContainer(Plugin plugin) : this() {
            ParentUID = plugin.UID;
            Properties = new List<Property>(plugin.GetProperties<Property>(true));
        }

        /// <summary>
        /// Gets or sets the parent uid.
        /// </summary>
        /// <value>
        /// The parent uid.
        /// </value>
        public string ParentUID { get; private set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public IEnumerable<Property> Properties { get; private set; }
    }
}