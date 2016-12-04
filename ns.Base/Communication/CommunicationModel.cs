using System;
using System.Runtime.Serialization;

namespace ns.Base.Communication {

    [DataContract]
    [KnownType(typeof(NodeCommunicationModel))]
    public class CommunicationModel : IIdentifiable {

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}