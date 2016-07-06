using System;

namespace ns.Base.Exceptions {

    public class ManagerInitializeException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerInitializeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ManagerInitializeException(string message) : base(message) {
        }
    }
}