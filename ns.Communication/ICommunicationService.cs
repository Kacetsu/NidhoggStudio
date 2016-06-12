using ns.Base.Plugins;
using ns.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication {

    [ServiceContract(SessionMode = SessionMode.Allowed)]
    internal interface ICommunicationService {

        [OperationContract]
        ProjectConfiguration GetProjectConfiguration();

        [OperationContract]
        List<Plugin> GetPlugins();
    }
}