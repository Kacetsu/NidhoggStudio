namespace ns.Base.Plugins.Properties {

    public interface ITolerance {

        /// <summary>
        /// Gets a value indicating whether [in tolerance].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [in tolerance]; otherwise, <c>false</c>.
        /// </value>
        bool InTolerance { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is tolerance enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tolerance enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsToleranceEnabled { get; }
    }

    public interface ITolerance<T> : ITolerance {

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        Tolerance<T> Tolerance { get; set; }
    }
}