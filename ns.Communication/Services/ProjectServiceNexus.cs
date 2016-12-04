using ns.Base;
using ns.Base.Log;
using ns.Base.Manager.ProjectBox;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services.Callbacks;
using ns.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ns.Communication.Services {

    internal class ProjectServiceNexus : Nexus<IProjectServiceCallbacks> {
        private static Lazy<ProjectServiceNexus> _instance = new Lazy<ProjectServiceNexus>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectServiceNexus"/> class.
        /// </summary>
        public ProjectServiceNexus() : base() {
        }

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentId">The parent identifier.</param>
        /// <exception cref="FaultException">Model is empty!
        /// or
        /// or</exception>
        public static void AddToolToProject(ToolModel model, Guid parentId) {
            if (model == null) {
                throw new FaultException("Model is empty!");
            }

            Operation operation = _instance.Value._projectManager.Configuration.Operations.Find(o => o.Id.Equals(parentId));
            if (operation == null) {
                throw new FaultException(string.Format("Could not find operation with UID {0}.", parentId));
            }

            Tool tool = _instance.Value._pluginManager.Items.Values.First(t => t.Fullname.Equals(model.Fullname)) as Tool;

            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", model.Fullname));
            }

            Tool copyTool = tool.Clone() as Tool;
            copyTool.Initialize();
            _instance.Value._projectManager.Add(copyTool, operation);

            // Notify clients.
            List<Guid> damagedIds = new List<Guid>();
            foreach (var client in _clients) {
                try {
                    client.Value?.OnToolAdded(new ToolModel(copyTool));
                } catch (CommunicationException ex) {
                    Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                    damagedIds.Add(client.Key);
                }
            }

            RemoveDisconnectedClients(damagedIds);
        }

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyId">The property identifier.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public static void ChangeListPropertySelectedIndex(int index, Guid propertyId) {
            if (Guid.Empty.Equals((propertyId))) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyId)));
            }

            Property property = _instance.Value._projectManager.FindProperty(propertyId);

            IValue valueProperty = property as IValue;
            IListProperty listProperty = property as IListProperty;
            IEnumerable<object> listObj = null;

            if (valueProperty != null && valueProperty.ValueObj.GetType().IsGenericType) {
                listObj = valueProperty.ValueObj as IEnumerable<object>;
            } else {
                throw new FaultException(string.Format("Property is not a generic list."));
            }

            if (property == null || listObj == null || listProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyId));
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
        /// <param name="propertyId">The property identifier.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public static void ChangePropertyValue(object newValue, Guid propertyId) {
            if (newValue == null) {
                throw new FaultException(string.Format("{0} is null.", nameof(newValue)));
            }

            if (Guid.Empty.Equals(propertyId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyId)));
            }

            Property property = _instance.Value._projectManager.FindProperty(propertyId);

            IValue valueProperty = property as IValue;

            if (property == null || valueProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyId));
            }

            try {
                valueProperty.ValueObj = newValue;
            } catch {
                throw new FaultException(string.Format("Property type mismatch. Property value type is [{0}] but new value is of type [{1}].", valueProperty.ValueObj.GetType(), newValue.GetType()));
            }

            // Notify clients.
            List<Guid> damagedIds = new List<Guid>();
            foreach (var client in _clients) {
                try {
                    client.Value?.OnPropertyChanged(propertyId);
                } catch (CommunicationException ex) {
                    Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                    damagedIds.Add(client.Key);
                }
            }

            RemoveDisconnectedClients(damagedIds);
        }

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetId">The target uid.</param>
        /// <param name="sourceId">The source identifier.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public static void ConnectProperties(Guid targetId, Guid sourceId) {
            if (Guid.Empty.Equals(targetId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(targetId)));
            }

            if (Guid.Empty.Equals(sourceId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(sourceId)));
            }

            Property targetProperty = _instance.Value._projectManager.FindProperty(targetId);
            Property sourceProperty = _instance.Value._projectManager.FindProperty(sourceId);

            if (targetProperty == null) {
                throw new FaultException(string.Format("Could not find target property!"));
            }

            if (sourceProperty == null) {
                throw new FaultException(string.Format("Could not find source property!"));
            }

            targetProperty.Connect(sourceProperty);
        }

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public static PropertyModel[] GetConnectableProperties(Guid propertyId) {
            if (Guid.Empty.Equals(propertyId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyId)));
            }

            Property targetProperty = _instance.Value._projectManager.FindProperty(propertyId);
            if (targetProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyId));
            }

            List<Property> connectableProperties = _instance.Value._projectManager.FindConnectableProperties(targetProperty);
            List<PropertyModel> connectableModels = new List<PropertyModel>();

            foreach (var property in connectableProperties) {
                connectableModels.Add(new PropertyModel(property));
            }

            return connectableModels.ToArray();
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <param name="id">The uid.</param>
        /// <returns></returns>
        public static OperationModel GetOperation(Guid id) => new OperationModel(_instance.Value._projectManager.Configuration.Operations.FirstOrDefault(o => o.Id.Equals(id)));

        /// <summary>
        /// Gets all operations.
        /// </summary>
        /// <returns></returns>
        public static OperationModel[] GetOperations() {
            List<OperationModel> result = new List<OperationModel>();

            foreach (Operation operation in _instance.Value._projectManager.Configuration.Operations) {
                result.Add(new OperationModel(operation));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <returns></returns>
        public static ProjectInfoContainer[] GetProjects() => _instance.Value._projectBoxManager.ProjectInfos.ToArray();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyId">The property uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public static PropertyModel GetProperty(Guid propertyId) {
            if (Guid.Empty.Equals(propertyId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyId)));
            }

            Property property = _instance.Value._projectManager.FindProperty(propertyId);
            if (property == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyId));
            }

            return new PropertyModel(property);
        }

        /// <summary>
        /// Gets the tool properties.
        /// </summary>
        /// <param name="toolId">The tool uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException"></exception>
        public static PropertyModel[] GetToolProperties(Guid toolId) {
            List<PropertyModel> properties = new List<PropertyModel>();

            Tool tool = _instance.Value._projectManager.FindTool(toolId);
            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", toolId));
            }

            foreach (Property property in tool.Items.Values.Where(p => p is Property)) {
                properties.Add(new PropertyModel(property));
            }

            return properties.ToArray();
        }

        public static void NewProject() {
        }

        /// <summary>
        /// Removes the tool from project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="FaultException">
        /// Model is empty!
        /// or
        /// or
        /// </exception>
        public static void RemoveToolFromProject(ToolModel model) {
            if (model == null) {
                throw new FaultException("Model is empty!");
            }

            Operation operation = _instance.Value._projectManager.Configuration.Operations.Find(o => o.Id.Equals(model.ParentId));
            if (operation == null) {
                throw new FaultException(string.Format("Could not find operation with ID {0}.", model.ParentId));
            }

            Tool tool = operation.Items[model.Id] as Tool;
            _instance.Value._projectManager.Remove(tool);

            // Notify clients.
            List<Guid> damagedIds = new List<Guid>();
            foreach (var client in _clients) {
                try {
                    client.Value?.OnToolRemoved(model);
                } catch (CommunicationException ex) {
                    Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                    damagedIds.Add(client.Key);
                }
            }

            RemoveDisconnectedClients(damagedIds);
        }

        /// <summary>
        /// Saves the project.
        /// </summary>
        public static void SaveProject() {
            ProcessorState lastState = CoreSystem.Instance.Processor.State;
            if (lastState == ProcessorState.Running) {
                CoreSystem.Instance.Processor.Stop();
            }

            _instance.Value._projectBoxManager.SaveProject();
            if (lastState == ProcessorState.Running) {
                CoreSystem.Instance.Processor.Start();
            }
        }
    }
}