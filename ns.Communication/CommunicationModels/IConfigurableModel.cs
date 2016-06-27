using ns.Communication.CommunicationModels.Properties;
using System.Collections.Generic;

namespace ns.Communication.CommunicationModels {

    public interface IConfigurableModel {

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        List<PropertyModel> Properties { get; }
    }
}