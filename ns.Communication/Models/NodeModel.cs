using ns.Base;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class NodeModel : GenericModel<Node>, IGenericModel<Node> {
    }
}