using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProjectServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProjectService {

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentId">The parent id.</param>
        [OperationContract(IsOneWay = true)]
        void AddToolToProject(ToolModel model, Guid parentId);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyId">The property id.</param>
        [OperationContract(IsOneWay = true)]
        void ChangeListPropertySelectedIndex(int index, Guid propertyId);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyId">The property id.</param>
        [OperationContract(IsOneWay = true)]
        void ChangePropertyValue(object newValue, Guid propertyId);

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetId">The target id.</param>
        /// <param name="sourceId">The source id.</param>
        [OperationContract(IsOneWay = true)]
        void ConnectProperties(Guid targetId, Guid sourceId);

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        [OperationContract]
        PropertyModel[] GetConnectableProperties(Guid propertyId);

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [OperationContract]
        OperationModel GetOperation(Guid id);

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        OperationModel[] GetOperations();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = false)]
        PropertyModel GetProperty(Guid propertyId);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract(IsInitiating = true, IsOneWay = true)]
        void RegisterClient(Guid id);

        /// <summary>
        /// Removes the tool from project.
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void RemoveToolFromProject(ToolModel model);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void UnregisterClient(Guid id);
    }
}