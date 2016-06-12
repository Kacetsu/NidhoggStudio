using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System.Runtime.Serialization;

namespace ns.Core.Configuration {

    [DataContract]
    public class ProjectConfiguration : IProjectConfiguration {

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [DataMember]
        public StringProperty FileName { get; set; } = new StringProperty();

        /// <summary>
        /// The project name
        /// </summary>
        [DataMember]
        public StringProperty Name { get; set; } = new StringProperty();

        /// <summary>
        /// The operations
        /// </summary>
        [DataMember]
        public ObservableList<Operation> Operations { get; set; } = new ObservableList<Operation>();
    }
}