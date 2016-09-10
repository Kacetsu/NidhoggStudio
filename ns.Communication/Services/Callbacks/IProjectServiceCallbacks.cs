using ns.Communication.Events;
using ns.Communication.Models;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    public interface IProjectServiceCallbacks : IToolEventHandlers {

        /// <summary>
        /// Called when [operation added].
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void OnOperationAdded(OperationModel model);

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="uid">The uid.</param>
        [OperationContract(IsOneWay = true)]
        void OnPropertyChanged(string uid);

        /// <summary>
        /// Called when [tool added].
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void OnToolAdded(ToolModel model);

        /// <summary>
        /// Called when [tool removed].
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void OnToolRemoved(ToolModel model);
    }
}