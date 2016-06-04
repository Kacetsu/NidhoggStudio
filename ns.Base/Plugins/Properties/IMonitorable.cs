namespace ns.Base.Plugins.Properties {

    public interface IMonitorable {

        /// <summary>
        /// Gets or sets a value indicating whether this instance is monitored.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is monitored; otherwise, <c>false</c>.
        /// </value>
        bool IsMonitored { get; set; }
    }
}