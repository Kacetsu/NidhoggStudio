using ns.Base.Manager;
using ns.Base.Plugins;
using ns.Core.Configuration;

namespace ns.Core.Manager {

    public interface IProjectManager : IGenericConfigurationManager<ProjectConfiguration> {

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        string FileName { get; set; }

        /// <summary>
        /// Adds the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        void Add(Operation operation);

        /// <summary>
        /// Adds the specified tool.
        /// </summary>
        /// <param name="tool">The tool.</param>
        /// <param name="parent">The parent.</param>
        void Add(Tool tool, Operation parent);

        /// <summary>
        /// Adds the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="parent">The parent.</param>
        void Add(Device device, Operation parent);

        /// <summary>
        /// Creates the default project.
        /// </summary>
        void CreateDefaultProject();
    }
}