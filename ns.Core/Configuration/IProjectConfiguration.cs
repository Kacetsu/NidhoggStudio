using ns.Base;
using ns.Base.Configuration;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Core.Configuration {

    public interface IProjectConfiguration : IBaseConfiguration {

        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        [DataMember]
        ObservableList<Operation> Operations { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [DataMember]
        StringProperty Name { get; set; }
    }
}