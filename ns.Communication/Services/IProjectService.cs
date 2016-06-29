﻿using ns.Communication.CommunicationModels;
using ns.Communication.Services.Callbacks;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services {

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IProjectServiceCallbacks)), ServiceKnownType(nameof(KnownTypesProvider.GetKnownTypes), typeof(KnownTypesProvider))]
    public interface IProjectService {

        /// <summary>
        /// Adds the tool to project.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="parentUID">The parent uid.</param>
        [OperationContract(IsOneWay = true)]
        void AddToolToProject(ToolModel model, string parentUID);

        /// <summary>
        /// Changes the index of the list property selected.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="propertyUID">The property uid.</param>
        [OperationContract]
        void ChangeListPropertySelectedIndex(int index, string propertyUID);

        /// <summary>
        /// Changes the property value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyUID">The property uid.</param>
        [OperationContract]
        void ChangePropertyValue(object newValue, string propertyUID);

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        OperationModel[] GetOperations();
    }
}