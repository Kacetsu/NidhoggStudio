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
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        [DataMember]
        public Property Property { get; protected set; }
    }
}