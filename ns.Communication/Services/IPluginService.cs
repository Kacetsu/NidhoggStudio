using ns.Communication.Models;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Allowed), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IPluginService {

        /// <summary>
        /// Gets the available plugins.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PluginModel> GetAvailablePlugins();

        /// <summary>
        /// Gets the available tools.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ToolModel> GetAvailableTools();
    }
}