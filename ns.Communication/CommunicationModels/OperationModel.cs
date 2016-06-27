using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.CommunicationModels.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels {

    [DataContract, KnownType(typeof(ToolModel)), KnownType(typeof(DevicePropertyModel))]
    public class OperationModel : GenericModel<Operation>, IGenericModel<Operation>, IOperationModel, IConfigurableModel {

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

        /// <summary>
        /// Gets the child tools.
        /// </summary>
        /// <value>
        /// The child tools.
        /// </value>
        [DataMember]
        public List<ToolModel> ChildTools { get; private set; } = new List<ToolModel>();

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [DataMember]
        public List<PropertyModel> Properties { get; private set; } = new List<PropertyModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationModel"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationModel(Operation operation) : base(operation) {
            DisplayName = operation.DisplayName;
            Description = operation.Description;
            Version = operation.Version;

            foreach (Tool tool in operation.Childs.Where(t => t is Tool)) {
                ChildTools.Add(new ToolModel(tool));
            }

            foreach (Property property in operation.Childs.Where(p => p is Property)) {
                if (property is DeviceProperty) {
                    Properties.Add(new DevicePropertyModel(property as DeviceProperty));
                } else {
                    Properties.Add(new PropertyModel(property));
                }
            }
        }
    }
}