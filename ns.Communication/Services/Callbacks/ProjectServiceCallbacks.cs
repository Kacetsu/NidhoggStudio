using ns.Communication.CommunicationModels;
using ns.Communication.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Services.Callbacks {

    public class ProjectServiceCallbacks : IProjectServiceCallbacks {

        /// <summary>
        /// Occurs when [tool added].
        /// </summary>
        public event ToolAddedEventHandler ToolAdded;

        /// <summary>
        /// Called when [operation added].
        /// </summary>
        /// <param name="model">The model.</param>
        public void OnOperationAdded(OperationModel model) {
        }

        /// <summary>
        /// Called when [tool added].
        /// </summary>
        /// <param name="model">The model.</param>
        public void OnToolAdded(ToolModel model) {
            ToolAdded?.Invoke(this, new CollectionChangedEventArgs(new List<ToolModel> { model }));
        }
    }
}