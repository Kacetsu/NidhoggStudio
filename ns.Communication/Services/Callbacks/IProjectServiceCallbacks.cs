using ns.Communication.CommunicationModels;
using ns.Communication.Events;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    public interface IProjectServiceCallbacks : IToolAddedEventHandler {

        /// <summary>
        /// Called when [operation added].
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void OnOperationAdded(OperationModel model);

        /// <summary>
        /// Called when [tool added].
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void OnToolAdded(ToolModel model);
    }
}