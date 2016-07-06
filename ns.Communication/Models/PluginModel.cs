using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class PluginModel : GenericModel<Plugin>, IGenericModel<Plugin>, IPluginModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginModel"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public PluginModel(Plugin plugin) : base(plugin) {
            DisplayName = plugin.DisplayName;
            Description = plugin.Description;
            Version = plugin.Version;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; private set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public string Version { get; private set; }
    }
}