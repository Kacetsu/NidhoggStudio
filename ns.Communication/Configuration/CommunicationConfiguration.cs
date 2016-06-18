using ns.Base.Configuration;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Communication.Configuration {

    [DataContract]
    public class CommunicationConfiguration : BaseConfiguration, IBaseConfiguration {

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        [DataMember]
        public StringProperty Address { get; set; } = new StringProperty(nameof(Address), "net.tcp://localhost:8080/");

        /// <summary>
        /// Gets the maximum size of the received message.
        /// </summary>
        /// <value>
        /// The maximum size of the received message.
        /// </value>
        public long MaxReceivedMessageSize { get; } = 67108864;
    }
}