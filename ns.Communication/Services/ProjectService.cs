using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.CommunicationModels;
using ns.Communication.CommunicationModels.Properties;
using ns.Communication.Services.Callbacks;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class ProjectService : IProjectService {
        private PluginManager _pluginManager;
        private ProjectManager _projectManager;

        public ProjectService() {
            _projectManager = CoreSystem.FindManager<ProjectManager>();
            _pluginManager = CoreSystem.FindManager<PluginManager>();
        }

        public IProjectServiceCallbacks Proxy { get { return OperationContext.Current.GetCallbackChannel<IProjectServiceCallbacks>(); } }

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
        public void AddToolToProject(ToolModel model, string parentUID) {
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

            // Notify client.
            Proxy.OnToolAdded(new ToolModel(copyTool));
        }

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyUID">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangeListPropertySelectedIndex(int index, string propertyUID) {
            if (string.IsNullOrEmpty(propertyUID)) {
                throw new FaultException(string.Format("{0} is null or empty.", nameof(propertyUID)));
            }

            Property property = _projectManager.FindProperty(propertyUID);

            IValue valueProperty = property as IValue;
            IListProperty listProperty = property as IListProperty;
            IEnumerable<object> listObj = null;

            if (valueProperty != null && valueProperty.ValueObj.GetType().IsGenericType) {
                listObj = valueProperty.ValueObj as IEnumerable<object>;
            } else {
                throw new FaultException(string.Format("Property is not a generic list."));
            }

            if (property == null || listObj == null || listProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyUID));
            }

            if (index > listObj.Count() - 1 || index < 0) {
                throw new FaultException(string.Format("Index out of bound."));
            }

            listProperty.SelectedObjItem = listObj.ElementAt(index);
        }

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyUID">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangePropertyValue(object newValue, string propertyUID) {
            if (newValue == null) {
                throw new FaultException(string.Format("{0} is null.", nameof(newValue)));
            }

            if (string.IsNullOrEmpty(propertyUID)) {
                throw new FaultException(string.Format("{0} is null or empty.", nameof(propertyUID)));
            }

            Property property = _projectManager.FindProperty(propertyUID);

            IValue valueProperty = property as IValue;

            if (property == null || valueProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyUID));
            }

            if (valueProperty.ValueObj.GetType() != newValue.GetType()) {
                throw new FaultException(string.Format("Property type mismatch. Property value type is [{0}] but new value is of type [{1}].", valueProperty.ValueObj.GetType(), newValue.GetType()));
            }

            valueProperty.ValueObj = newValue;
        }

        /// <summary>
        /// Gets all operations.
        /// </summary>
        /// <returns></returns>
        public OperationModel[] GetOperations() {
            List<OperationModel> result = new List<OperationModel>();

            foreach (Operation operation in _projectManager.Configuration.Operations) {
                result.Add(new OperationModel(operation));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the tool properties.
        /// </summary>
        /// <param name="toolUID">The tool uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException"></exception>
        public PropertyModel[] GetToolProperties(string toolUID) {
            List<PropertyModel> properties = new List<PropertyModel>();

            Tool tool = _projectManager.FindTool(toolUID);
            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", toolUID));
            }

            foreach (Property property in tool.Childs.Where(p => p is Property)) {
                properties.Add(new PropertyModel(property));
            }

            return properties.ToArray();
        }
    }
}