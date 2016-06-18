using ns.Communication.CommunicationModels;
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
        List<PluginCommunicationModel> GetAvailablePlugins();

        /// <summary>
        /// Gets the available tools.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ToolCommunicationModel> GetAvailableTools();
    }
}