using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProjectServiceClient : GenericDuplexServiceClient<IProjectService, IProjectServiceCallbacks>, IProjectService {
        private string _clientUID = Guid.NewGuid().ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        public ProjectServiceClient(EndpointAddress endpoint, Binding binding, ProjectServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
            RegisterClient(_clientUID);
        }

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        public void AddToolToProject(ToolModel model, string parentUID) => Channel?.AddToolToProject(model, parentUID);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyUID">The property uid.</param>
        public void ChangeListPropertySelectedIndex(int index, string propertyUID) => Channel?.ChangeListPropertySelectedIndex(index, propertyUID);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyUID">The property uid.</param>
        public void ChangePropertyValue(object newValue, string propertyUID) => Channel?.ChangePropertyValue(newValue, propertyUID);

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetUID">The target uid.</param>
        /// <param name="sourceUID">The source uid.</param>
        public void ConnectProperties(string targetUID, string sourceUID) => Channel?.ConnectProperties(targetUID, sourceUID);

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="propertyUID"></param>
        /// <returns></returns>
        public PropertyModel[] GetConnectableProperties(string propertyUID) => Channel?.GetConnectableProperties(propertyUID);

        /// <summary>
        /// Gets the project operations.
        /// </summary>
        /// <returns></returns>
        public OperationModel[] GetOperations() => Channel?.GetOperations();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyUID">The property uid.</param>
        /// <returns></returns>
        public PropertyModel GetProperty(string propertyUID) => Channel?.GetProperty(propertyUID);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void RegisterClient(string uid) => Channel?.RegisterClient(uid);

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        public void UnregisterClient(string uid) => Channel?.UnregisterClient(uid);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing) {
            UnregisterClient(_clientUID);
            base.Dispose(disposing);
        }
    }
}