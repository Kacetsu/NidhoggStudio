using ns.Base.Collections;
using ns.Base.Configuration;
using ns.Base.Plugins;
using ns.Base.Plugins.Devices;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Core.Configuration {

    public interface IProjectConfiguration : IBaseConfiguration {

        /// <summary>
        /// Gets or sets the devices.
        /// </summary>
        /// <value>
        /// The devices.
        /// </value>
        [DataMember]
        ObservableList<Device> Devices { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>
        /// The name of the project.
        /// </value>
        [DataMember]
        StringProperty Name { get; set; }

        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        [DataMember]
        ObservableList<Operation> Operations { get; set; }
    }
}