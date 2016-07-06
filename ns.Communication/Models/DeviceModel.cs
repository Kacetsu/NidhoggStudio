using ns.Base.Plugins;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class DeviceModel : GenericModel<Device>, IGenericModel<Device> {

        public DeviceModel(Device device) : base(device) {
        }
    }
}