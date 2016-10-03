using ns.Base.Manager.ProjectBox;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using System;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class ProjectService : IProjectService {

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentId">The parent uid.</param>
        /// <exception cref="FaultException">
        /// Model is empty!
        /// or
        /// or
        /// </exception>
        public void AddToolToProject(ToolModel model, Guid parentId) => ProjectServiceNexus.AddToolToProject(model, parentId);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyId">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangeListPropertySelectedIndex(int index, Guid propertyId) => ProjectServiceNexus.ChangeListPropertySelectedIndex(index, propertyId);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyId">The property uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ChangePropertyValue(object newValue, Guid propertyId) => ProjectServiceNexus.ChangePropertyValue(newValue, propertyId);

        /// <summary>
        /// Connects the properties.
        /// </summary>
        /// <param name="targetId">The target uid.</param>
        /// <param name="sourceId">The source uid.</param>
        /// <exception cref="FaultException">
        /// </exception>
        public void ConnectProperties(Guid targetId, Guid sourceId) => ProjectServiceNexus.ConnectProperties(targetId, sourceId);

        /// <summary>
        /// Gets the connectable properties.
        /// </summary>
        /// <param name="propertyId">The property uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public PropertyModel[] GetConnectableProperties(Guid propertyId) => ProjectServiceNexus.GetConnectableProperties(propertyId);

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <param name="id">The uid.</param>
        /// <returns></returns>
        public OperationModel GetOperation(Guid id) => ProjectServiceNexus.GetOperation(id);

        /// <summary>
        /// Gets all operations.
        /// </summary>
        /// <returns></returns>
        public OperationModel[] GetOperations() => ProjectServiceNexus.GetOperations();

        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <returns></returns>
        public ProjectInfoContainer[] GetProjects() => ProjectServiceNexus.GetProjects();

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyId">The property uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException">
        /// </exception>
        public PropertyModel GetProperty(Guid propertyId) => ProjectServiceNexus.GetProperty(propertyId);

        /// <summary>
        /// Gets the tool properties.
        /// </summary>
        /// <param name="toolId">The tool uid.</param>
        /// <returns></returns>
        /// <exception cref="FaultException"></exception>
        public PropertyModel[] GetToolProperties(Guid toolId) => ProjectServiceNexus.GetToolProperties(toolId);

        /// <summary>
        /// Registers the client.
        /// </summary>
        /// <param name="id">The uid.</param>
        /// <exception cref="FaultException"></exception>
        public void RegisterClient(Guid id) => ProjectServiceNexus.RegisterClient(id);

        /// <summary>
        /// Removes the tool from project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="FaultException">
        /// Model is empty!
        /// or
        /// or
        /// </exception>
        public void RemoveToolFromProject(ToolModel model) => ProjectServiceNexus.RemoveToolFromProject(model);

        /// <summary>
        /// Saves the project.
        /// </summary>
        public void SaveProject() => ProjectServiceNexus.SaveProject();

        /// <summary>
        /// Unregisters the client.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void UnregisterClient(Guid id) => ProjectServiceNexus.UnregisterClient(id);
    }
}