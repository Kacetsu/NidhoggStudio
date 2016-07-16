using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Communication.Models.Properties {

    [DataContract]
    public class PropertyModel : GenericModel<Property> {

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyModel"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public PropertyModel(Property property) : base(property) {
            Property = property;
            TreeName = property.TreeName;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        [DataMember]
        public Property Property { get; protected set; }

        /// <summary>
        /// Gets the name of the tree.
        /// </summary>
        /// <value>
        /// The name of the tree.
        /// </value>
        [DataMember]
        public string TreeName { get; private set; }
    }
}