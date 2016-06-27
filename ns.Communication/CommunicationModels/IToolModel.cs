using ns.Base.Plugins;

namespace ns.Communication.CommunicationModels {

    public interface IToolModel : IPluginModel, ISelectableModel {

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        string Category { get; }

        /// <summary>
        /// Gets the parent uid.
        /// </summary>
        /// <value>
        /// The parent uid.
        /// </value>
        string ParentUID { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        PluginStatus Status { get; }
    }
}