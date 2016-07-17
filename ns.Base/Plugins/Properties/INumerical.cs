namespace ns.Base.Plugins.Properties {

    public interface INumerical {

        /// <summary>
        /// Gets a value indicating whether this instance is numeric.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is numeric; otherwise, <c>false</c>.
        /// </value>
        bool IsNumeric { get; }
    }

    public interface INumerical<T> : INumerical {

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        T Max { get; set; }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        T Min { get; set; }
    }
}