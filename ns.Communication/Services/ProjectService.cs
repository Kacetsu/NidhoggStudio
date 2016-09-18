using ns.Base.Log;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.Communication.Services.Callbacks;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ProjectService : IProjectService {
        private Dictionary<Guid, IProjectServiceCallbacks> _clients = new Dictionary<Guid, IProjectServiceCallbacks>();
        private PluginManager _pluginManager;
        private ProjectManager _projectManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        public ProjectService() {
            _projectManager = CoreSystem.FindManager<ProjectManager>();
            _pluginManager = CoreSystem.FindManager<PluginManager>();
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
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
        public void AddToolToProject(ToolModel model, Guid parentUID) {
            if (model == null) {
                throw new FaultException("Model is empty!");
            }

            Operation operation = _projectManager.Configuration.Operations.Find(o => o.Id.Equals(parentUID));
            if (operation == null) {
                throw new FaultException(string.Format("Could not find operation with UID {0}.", parentUID));
            }

            Tool tool = _pluginManager.Nodes.First(t => t.Fullname.Equals(model.Fullname)) as Tool;

            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", model.Fullname));
            }

            Tool copyTool = tool.Clone() as Tool;
            _projectManager.Add(copyTool, operation);

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

            foreach (Guid damagedId in damagedIds) {
                _clients.Remove(damagedId);
            }
        }

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyUID">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangeListPropertySelectedIndex(int index, Guid propertyUID) {
            if (Guid.Empty.Equals((propertyUID))) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyUID)));
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
        /// <param name="propertyId">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangePropertyValue(object newValue, Guid propertyId) {
            if (newValue == null) {
                throw new FaultException(string.Format("{0} is null.", nameof(newValue)));
            }

            if (Guid.Empty.Equals(propertyId)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyId)));
            }

            Property property = _projectManager.FindProperty(propertyId);

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

            foreach (Guid damagedId in damagedIds) {
                _clients.Remove(damagedId);
            }
        }

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetUID">The target uid.</param>
        /// <param name="sourceUID">The source uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ConnectProperties(Guid targetUID, Guid sourceUID) {
            if (Guid.Empty.Equals(targetUID)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(targetUID)));
            }

            if (Guid.Empty.Equals(sourceUID)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(sourceUID)));
            }

            Property targetProperty = _projectManager.FindProperty(targetUID);
            Property sourceProperty = _projectManager.FindProperty(sourceUID);

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
        /// <param name="propertyUID">The property uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public PropertyModel[] GetConnectableProperties(Guid propertyUID) {
            if (Guid.Empty.Equals(propertyUID)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyUID)));
            }

            Property targetProperty = _projectManager.FindProperty(propertyUID);
            if (targetProperty == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyUID));
            }

            List<Property> connectableProperties = _projectManager.FindConnectableProperties(targetProperty);
            List<PropertyModel> connectableModels = new List<PropertyModel>();

            foreach (var property in connectableProperties) {
                connectableModels.Add(new PropertyModel(property));
            }

            return connectableModels.ToArray();
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public OperationModel GetOperation(Guid uid) {
            OperationModel model = new OperationModel(_projectManager.Configuration.Operations.FirstOrDefault(o => o.Id.Equals(uid)));
            return model;
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
        /// Gets the property.
        /// </summary>
        /// <param name="propertyUID">The property uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public PropertyModel GetProperty(Guid propertyUID) {
            if (Guid.Empty.Equals(propertyUID)) {
                throw new FaultException(string.Format("{0} is empty.", nameof(propertyUID)));
            }

            Property property = _projectManager.FindProperty(propertyUID);
            if (property == null) {
                throw new FaultException(string.Format("Could not find property {0}.", propertyUID));
            }

            return new PropertyModel(property);
        }

        /// <summary>
        /// Gets the tool properties.
        /// </summary>
        /// <param name="toolUID">The tool uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException"></exception>
        public PropertyModel[] GetToolProperties(Guid toolUID) {
            List<PropertyModel> properties = new List<PropertyModel>();

            Tool tool = _projectManager.FindTool(toolUID);
            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", toolUID));
            }

            foreach (Property property in tool.Items.Where(p => p is Property)) {
                properties.Add(new PropertyModel(property));
            }

            return properties.ToArray();
        }

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid uid) {
            if (_clients.ContainsKey(uid)) {
                throw new FaultException(string.Format("Client {0} already exists!", uid));
            }

            _clients.Add(uid, OperationContext.Current.GetCallbackChannel<IProjectServiceCallbacks>());
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
        public void RemoveToolFromProject(ToolModel model) {
            if (model == null) {
                throw new FaultException("Model is empty!");
            }

            Operation operation = _projectManager.Configuration.Operations.Find(o => o.Id.Equals(model.ParentId));
            if (operation == null) {
                throw new FaultException(string.Format("Could not find operation with UID {0}.", model.ParentId));
            }

            Tool tool = operation.Items.Find(t => t.Id.Equals(model.Id)) as Tool;

            if (tool == null) {
                throw new FaultException(string.Format("Could not find tool {0}.", model.Fullname));
            }

            _projectManager.Remove(tool);

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

            foreach (Guid damagedId in damagedIds) {
                _clients.Remove(damagedId);
            }
        }

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(Guid id) => _clients?.Remove(id);
    }
}