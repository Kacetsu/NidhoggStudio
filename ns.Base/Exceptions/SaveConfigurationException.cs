using System;

namespace ns.Base.Exceptions {

    [Serializable]
    public class SaveConfigurationException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveConfigurationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SaveConfigurationException(string message) : base(message) {
        }
    }
}