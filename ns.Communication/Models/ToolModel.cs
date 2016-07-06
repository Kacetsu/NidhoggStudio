using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.Models.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class ToolModel : GenericModel<Tool>, IGenericModel<Tool>, IToolModel, IConfigurableModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolModel"/> class.
        /// </summary>
        /// <param name="tool">The tool.</param>
        public ToolModel(Tool tool) : base(tool) {
            Category = tool.Category;
            Status = tool.Status;
            DisplayName = tool.DisplayName;
            Description = tool.Description;
            Version = tool.Version;
            ParentUID = tool.Parent?.UID;

            foreach (Property property in tool.Childs.Where(p => p is Property)) {
                Properties.Add(new PropertyModel(property));
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [DataMember]
        public string Category { get; private set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the parent uid.
        /// </summary>
        /// <value>
        /// The parent uid.
        /// </value>
        [DataMember]
        public string ParentUID { get; private set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [DataMember]
        public List<PropertyModel> Properties { get; private set; } = new List<PropertyModel>();

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
    }
}