using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace ns.Base.Communication {

    [DataContract]
    public class NodeCommunicationModel : CommunicationModel {

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCommunicationModel"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public NodeCommunicationModel(Node node)
            : base() {
            foreach (var item in node.Items) {
                Items.TryAdd(item.Value.CommunicationModel as NodeCommunicationModel);
            }
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [DataMember]
        public BlockingCollection<NodeCommunicationModel> Items { get; } = new BlockingCollection<NodeCommunicationModel>();
    }
}