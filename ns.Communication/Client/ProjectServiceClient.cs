using ns.Base.Manager.ProjectBox;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProjectServiceClient : GenericDuplexServiceClient<IProjectService, IProjectServiceCallbacks>, IProjectService {
        private Guid _clientId = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        public ProjectServiceClient(EndpointAddress endpoint, Binding binding, ProjectServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(_clientId);
        }

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentId">The parent id.</param>
        public void AddToolToProject(ToolModel model, Guid parentId) => Channel?.AddToolToProject(model, parentId);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyId">The property id.</param>
        public void ChangeListPropertySelectedIndex(int index, Guid propertyId) => Channel?.ChangeListPropertySelectedIndex(index, propertyId);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyId">The property id.</param>
        public void ChangePropertyValue(object newValue, Guid propertyId) => Channel?.ChangePropertyValue(newValue, propertyId);

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetId">The target id.</param>
        /// <param name="sourceId">The source id.</param>
        public void ConnectProperties(Guid targetId, Guid sourceId) => Channel?.ConnectProperties(targetId, sourceId);

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public PropertyModel[] GetConnectableProperties(Guid propertyId) => Channel?.GetConnectableProperties(propertyId);

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public OperationModel GetOperation(Guid id) => Channel?.GetOperation(id);

        /// <summary>
        /// Gets the project operations.
        /// </summary>
        /// <returns></returns>
        public OperationModel[] GetOperations() => Channel?.GetOperations();

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <returns></returns>
        public ProjectInfoContainer[] GetProjects() => Channel?.GetProjects();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyId">The property id.</param>
        /// <returns></returns>
        public PropertyModel GetProperty(Guid propertyId) => Channel?.GetProperty(propertyId);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The id.</param>
        public void RegisterClient(Guid id) => Channel?.RegisterClient(id);

        /// <summary>
        /// Removes the tool from project.
        /// </summary>
        /// <param name="model">The model.</param>
        public void RemoveToolFromProject(ToolModel model) => Channel?.RemoveToolFromProject(model);

        /// <summary>
        /// Saves the project.
        /// </summary>
        public void SaveProject() => Channel?.SaveProject();

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        public void UnregisterClient(Guid id) => Channel?.UnregisterClient(id);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            UnregisterClient(_clientId);
            base.Dispose(disposing);
        }
    }
}