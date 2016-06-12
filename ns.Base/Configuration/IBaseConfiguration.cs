using ns.Base.Plugins.Properties;

namespace ns.Base.Configuration {

    public interface IBaseConfiguration {

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        StringProperty FileName { get; set; }
    }
}