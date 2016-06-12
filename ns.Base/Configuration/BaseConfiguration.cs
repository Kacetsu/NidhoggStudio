using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Base.Configuration {

    public class BaseConfiguration : IBaseConfiguration {

        /// <summary>
        /// The project name
        /// </summary>
        [DataMember]
        public StringProperty FileName { get; set; } = new StringProperty();
    }
}