using ns.Communication.CommunicationModels;
using ns.Communication.Services.Callbacks;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProjectServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProjectService {

        [OperationContract]
        OperationModel[] GetOperations();

        [OperationContract(IsOneWay = true)]
        void AddToolToProject(ToolModel model, string parentUID);

        [OperationContract]
        void ChangePropertyValue(object newValue, string propertyUID);
    }
}