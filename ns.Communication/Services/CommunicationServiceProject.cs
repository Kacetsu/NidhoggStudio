using ns.Base.Plugins;
using ns.Communication.CommunicationModels;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    public partial class CommunicationService : IProjectService {

        /// <summary>
        /// Gets all operations.
        /// </summary>
        /// <returns></returns>
        public List<OperationCommunicationModel> GetProjectOperations() {
            List<OperationCommunicationModel> result = new List<OperationCommunicationModel>();

            foreach (Operation operation in _projectManager.Configuration.Operations) {
                result.Add(new OperationCommunicationModel(operation));
            }

            return result;
        }

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        /// <exception cref="FaultException">
        /// Model is empty!
        /// or
        /// or
        /// </exception>
        public void AddToolToProject(ToolCommunicationModel model, string parentUID) {
            if (model == null) {
                throw new FaultException("Model is empty!");
            }

            Operation operation = _projectManager.Configuration.Operations.Find(o => o.UID.Equals(parentUID));
            if (operation == null) {
                throw new FaultException(string.Format("Could not find operation with UID {0}.", parentUID));
            }

            Tool tool = _pluginManager.Nodes.Find(t => t.Fullname.Equals(model.Fullname)) as Tool;

            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", model.Fullname));
            }

            Tool copyTool = new Tool(tool);
            operation.AddChild(copyTool);
        }
    }
}