namespace ns.Base.Manager {

    public interface IManager {

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        bool Finalize();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        bool Initialize();
    }
}