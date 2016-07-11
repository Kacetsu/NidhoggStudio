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
        /// Gets the data storage service address.
        /// </summary>
        /// <value>
        /// The data storage service address.
        /// </value>
        public string DataStorageServiceAddress { get { return string.Format("{0}DataStorage", Address.Value); } }

        /// <summary>
        /// Gets the maximum size of the received message.
        /// </summary>
        /// <value>
        /// The maximum size of the received message.
        /// </value>
        public long MaxReceivedMessageSize { get; } = 67108864;

        /// <summary>
        /// Gets the plugin service address.
        /// </summary>
        /// <value>
        /// The plugin service address.
        /// </value>
        public string PluginServiceAddress { get { return string.Format("{0}Plugin", Address.Value); } }

        /// <summary>
        /// Gets the processor service address.
        /// </summary>
        /// <value>
        /// The processor service address.
        /// </value>
        public string ProcessorServiceAddress { get { return string.Format("{0}Processor", Address.Value); } }

        /// <summary>
        /// Gets the project service address.
        /// </summary>
        /// <value>
        /// The project service address.
        /// </value>
        public string ProjectServiceAddress { get { return string.Format("{0}Project", Address.Value); } }

        /// <summary>
        /// Gets the receive timeout.
        /// </summary>
        /// <value>
        /// The receive timeout.
        /// </value>
        public int ReceiveTimeout { get; } = 10000;
    }
}