using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class DeviceCommunicationModel : GenericCommunicationModel<Device>, IGenericCommunicationModel<Device> {

        public DeviceCommunicationModel(Device device) : base(device) {
        }
    }
}