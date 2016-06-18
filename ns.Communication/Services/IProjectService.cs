using ns.Communication.CommunicationModels;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Allowed), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProjectService {

        [OperationContract]
        List<OperationCommunicationModel> GetProjectOperations();

        [OperationContract]
        void AddToolToProject(ToolCommunicationModel model, string parentUID);
    }
}