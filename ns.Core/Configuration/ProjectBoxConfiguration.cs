using ns.Base.Configuration;
using System.Runtime.Serialization;

namespace ns.Core.Configuration {

    [DataContract]
    public class ProjectBoxConfiguration : BaseConfiguration {

        /// <summary>
        /// Gets or sets the last used project path.
        /// </summary>
        /// <value>
        /// The last used project path.
        /// </value>
        [DataMember]
        public string LastUsedProjectPath { get; set; }
    }
}