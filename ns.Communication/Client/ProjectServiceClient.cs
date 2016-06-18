using ns.Communication.CommunicationModels;
using ns.Communication.Services;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class ProjectServiceClient : GenericServiceClient<IProjectService>, IProjectService {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceClient"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        public ProjectServiceClient(EndpointAddress endpoint, Binding binding) : base(endpoint, binding) {
        }

        /// <summary>
        /// Gets the project operations.
        /// </summary>
        /// <returns></returns>
        public List<OperationCommunicationModel> GetProjectOperations() => Channel?.GetProjectOperations();

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        public void AddToolToProject(ToolCommunicationModel model, string parentUID) => Channel?.AddToolToProject(model, parentUID);
    }
}