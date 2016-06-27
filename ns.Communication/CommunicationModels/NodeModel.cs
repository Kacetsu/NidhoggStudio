using ns.Base;
using System.Runtime.Serialization;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class NodeModel : GenericModel<Node>, IGenericModel<Node> {
    }
}