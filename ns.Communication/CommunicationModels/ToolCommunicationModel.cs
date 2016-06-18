using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class ToolCommunicationModel : GenericCommunicationModel<Tool>, IGenericCommunicationModel<Tool>, IPluginCommunicationModel, ISelectable {

        /// <summary>
        /// Gets the display name.
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
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [DataMember]
        public string Category { get; private set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [DataMember]
        public PluginStatus Status { get; private set; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public string Version { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolCommunicationModel"/> class.
        /// </summary>
        /// <param name="tool">The tool.</param>
        public ToolCommunicationModel(Tool tool) : base(tool) {
            Category = tool.Category;
            Status = tool.Status;
            DisplayName = tool.DisplayName;
            Description = tool.Description;
            Version = tool.Version;
        }
    }
}