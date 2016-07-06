using System.Collections.Generic;

namespace ns.Communication.Models {

    public interface IOperationModel : IPluginModel, ISelectableModel {

        /// <summary>
        /// Gets the child tools.
        /// </summary>
        /// <value>
        /// The child tools.
        /// </value>
        List<ToolModel> ChildTools { get; }
    }
}