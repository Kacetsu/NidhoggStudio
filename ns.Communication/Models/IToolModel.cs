using ns.Base.Plugins;
using System;

namespace ns.Communication.Models {

    public interface IToolModel : IPluginModel, ISelectableModel {

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        string Category { get; }

        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        Guid ParentId { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        PluginStatus Status { get; }
    }
}