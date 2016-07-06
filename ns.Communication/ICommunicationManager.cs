namespace ns.Communication {

    public interface ICommunicationManager {

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        void Connect();
    }
}