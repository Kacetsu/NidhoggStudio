using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services.Callbacks;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProjectServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProjectService {

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        [OperationContract(IsOneWay = true)]
        void AddToolToProject(ToolModel model, string parentUID);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyUID">The property uid.</param>
        [OperationContract(IsOneWay = true)]
        void ChangeListPropertySelectedIndex(int index, string propertyUID);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyUID">The property uid.</param>
        [OperationContract(IsOneWay = true)]
        void ChangePropertyValue(object newValue, string propertyUID);

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetUID">The target uid.</param>
        /// <param name="sourceUID">The source uid.</param>
        [OperationContract]
        void ConnectProperties(string targetUID, string sourceUID);

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        [OperationContract]
        PropertyModel[] GetConnectableProperties(string propertyUID);

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        OperationModel[] GetOperations();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyUID">The property uid.</param>
        /// <returns></returns>
        [OperationContract]
        PropertyModel GetProperty(string propertyUID);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        [OperationContract]
        void RegisterClient(string uid);

        /// <summary>
        /// Removes the tool from project.
        /// </summary>
        /// <param name="model">The model.</param>
        [OperationContract(IsOneWay = true)]
        void RemoveToolFromProject(ToolModel model);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        [OperationContract]
        void UnregisterClient(string uid);
    }
}