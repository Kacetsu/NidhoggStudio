using ns.Communication.Events;
using ns.Communication.Models;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    public interface IProcessorServiceCallbacks : IProcessorStateChangedEventHandler {

        /// <summary>
        /// Called when [processor started].
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void OnProcessorStarted(ProcessorInfoModel processorInfoModel);

        /// <summary>
        /// Called when [processor stopped].
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void OnProcessorStopped(ProcessorInfoModel processorInfoModel);
    }
}