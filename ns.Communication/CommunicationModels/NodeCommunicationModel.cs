using ns.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class NodeCommunicationModel : GenericCommunicationModel<Node>, IGenericCommunicationModel<Node> {
    }
}