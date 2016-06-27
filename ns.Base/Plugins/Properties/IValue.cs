namespace ns.Base.Plugins.Properties {

    public interface IValue {

        /// <summary>
        /// Gets the value object.
        /// </summary>
        /// <value>
        /// The value object.
        /// </value>
        object ValueObj { get; set; }
    }

    public interface IValue<T> : IValue {

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        T Value { get; set; }
    }
}