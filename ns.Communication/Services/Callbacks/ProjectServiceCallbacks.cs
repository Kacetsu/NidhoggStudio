using ns.Communication.Events;
using ns.Communication.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ns.Communication.Services.Callbacks {

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ProjectServiceCallbacks : IProjectServiceCallbacks {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when [tool added].
        /// </summary>
        public event ToolAddedEventHandler ToolAdded;

        /// <summary>
        /// Occurs when [tool removed].
        /// </summary>
        public event ToolRemovedEventHandler ToolRemoved;

        /// <summary>
        /// Called when [operation added].
        /// </summary>
        /// <param name="model">The model.</param>
        public void OnOperationAdded(OperationModel model) {
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="id">The id.</param>
        public void OnPropertyChanged(Guid id) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(id));

        /// <summary>
        /// Called when [tool added].
        /// </summary>
        /// <param name="model">The model.</param>
        public void OnToolAdded(ToolModel model) => ToolAdded?.Invoke(this, new CollectionChangedEventArgs(new List<ToolModel> { model }));

        /// <summary>
        /// Called when [tool removed].
        /// </summary>
        /// <param name="model">The model.</param>
        public void OnToolRemoved(ToolModel model) => ToolRemoved?.Invoke(this, new CollectionChangedEventArgs(new List<ToolModel> { model }));
    }
}