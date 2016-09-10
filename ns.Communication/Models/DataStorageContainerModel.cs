using ns.Base.Manager.DataStorage;
using ns.Base.Plugins.Properties;
using ns.Communication.Models.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public class DataStorageContainerModel : GenericModel<DataContainer> {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainerModel"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DataStorageContainerModel(DataContainer container) : base(container) {
            UID = container.ParentUID;
            foreach (Property property in container.Properties) {
                Properties.Add(new PropertyModel(property));
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [DataMember]
        public List<PropertyModel> Properties { get; private set; } = new List<PropertyModel>();
    }
}