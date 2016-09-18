using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ns.Base.Manager.DataStorage {

    public abstract class DataContainer : Node {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContainer"/> class.
        /// </summary>
        protected DataContainer() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContainer"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        protected DataContainer(Plugin plugin) : this() {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));

            ParentId = plugin.Id;
            Properties = new List<Property>(plugin.GetProperties<Property>(true));

            ImageProperty outImageProperty = Properties.FirstOrDefault(p => p is ImageProperty) as ImageProperty;
            if (outImageProperty == null) {
                ImageProperty inImageProperty = null;
                if (plugin.TryGetProperty(out inImageProperty)) {
                    Properties.Add(inImageProperty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent uid.
        /// </summary>
        /// <value>
        /// The parent uid.
        /// </value>
        public Guid ParentId { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public ICollection<Property> Properties { get; }
    }
}