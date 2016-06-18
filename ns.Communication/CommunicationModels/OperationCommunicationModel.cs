using ns.Base.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class OperationCommunicationModel : GenericCommunicationModel<Operation>, IGenericCommunicationModel<Operation>, IPluginCommunicationModel, ISelectable {

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

        [DataMember]
        public List<ToolCommunicationModel> ChildTools { get; } = new List<ToolCommunicationModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationCommunicationModel"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationCommunicationModel(Operation operation) : base(operation) {
            DisplayName = operation.DisplayName;
            Description = operation.Description;
            Version = operation.Version;

            foreach (Tool tool in operation.Childs.Where(t => t is Tool)) {
                ChildTools.Add(new ToolCommunicationModel(tool));
            }
        }
    }
}