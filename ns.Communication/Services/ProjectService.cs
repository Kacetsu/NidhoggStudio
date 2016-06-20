using ns.Base.Plugins;
using ns.Communication.CommunicationModels;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class ProjectService : IProjectService {
        private ProjectManager _projectManager;
        private PluginManager _pluginManager;

        public INotificationServiceCallbacks Proxy { get { return OperationContext.Current.GetCallbackChannel<INotificationServiceCallbacks>(); } }

        public ProjectService() {
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
            _pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
        }

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
            Proxy.OnToolAdded(new ToolCommunicationModel(copyTool));
        }
    }
}