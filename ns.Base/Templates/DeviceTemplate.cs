﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Templates {

    [DataContract]
    [KnownType(typeof(ImageDeviceTemplate))]
    public abstract class DeviceTemplate : Template {
    }
}