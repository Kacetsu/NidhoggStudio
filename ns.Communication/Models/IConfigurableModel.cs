using ns.Communication.Models.Properties;
using System.Collections.Generic;

namespace ns.Communication.Models {

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