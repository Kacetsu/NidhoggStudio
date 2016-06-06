using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.ObjectModel;

namespace ns.Core.Configuration {

    [Serializable]
    public class ProjectConfiguration {

        /// <summary>
        /// The project name
        /// </summary>
        public StringProperty ProjectName { get; set; } = new StringProperty();

        /// <summary>
        /// The operations
        /// </summary>
        public ObservableCollection<Operation> Operations { get; set; } = new ObservableCollection<Operation>();
    }
}