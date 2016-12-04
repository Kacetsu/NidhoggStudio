using ns.Base.Plugins.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {

    [DataContract]
    public class DeviceContainerProperty : GenericProperty<Device> {
    }
}