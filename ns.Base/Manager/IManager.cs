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
        /// Closes this instance.
        /// </summary>
        void Close();
    }
}