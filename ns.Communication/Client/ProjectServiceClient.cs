using ns.Communication.CommunicationModels;
using ns.Communication.Services;
using ns.Communication.Services.Callbacks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProjectServiceClient : GenericDuplexServiceClient<IProjectService, IProjectServiceCallbacks>, IProjectService {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        public ProjectServiceClient(EndpointAddress endpoint, Binding binding, ProjectServiceCallbacks callbacks) : base(endpoint, binding, callbacks) {
        }

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        public void AddToolToProject(ToolModel model, string parentUID) => Channel?.AddToolToProject(model, parentUID);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyUID">The property uid.</param>
        public void ChangePropertyValue(object newValue, string propertyUID) => Channel?.ChangePropertyValue(newValue, propertyUID);

        /// <summary>
        /// Gets the project operations.
        /// </summary>
        /// <returns></returns>
        public OperationModel[] GetOperations() => Channel?.GetOperations();
    }
}