﻿using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class PluginCommunicationModel : GenericCommunicationModel<Plugin>, IGenericCommunicationModel<Plugin>, IPluginCommunicationModel {

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public string Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginCommunicationModel"/> class.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        public PluginCommunicationModel(Plugin plugin) : base(plugin) {
            DisplayName = plugin.DisplayName;
            Description = plugin.Description;
            Version = plugin.Version;
        }
    }
}