using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ns.Core.Configuration {

    [Serializable, DataContract]
    public class ProjectConfiguration {

        /// <summary>
        /// The project name
        /// </summary>
        [DataMember]
        public StringProperty ProjectName { get; set; } = new StringProperty();

        /// <summary>
        /// The operations
        /// </summary>
        [DataMember]
        public ObservableCollection<Operation> Operations { get; set; } = new ObservableCollection<Operation>();
    }
}