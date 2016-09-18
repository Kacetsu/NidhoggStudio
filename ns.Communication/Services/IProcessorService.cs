using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProcessorServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProcessorService {

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ProcessorInfoModel GetState();

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract]
        void RegisterClient(Guid id);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Stop();

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract]
        void UnregisterClient(Guid id);
    }
}