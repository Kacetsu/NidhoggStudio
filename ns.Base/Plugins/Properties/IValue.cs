namespace ns.Base.Plugins.Properties {

    public interface IValue<T> {

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        T Value { get; set; }
    }
}