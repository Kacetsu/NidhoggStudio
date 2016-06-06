using System;

namespace ns.Base.Exceptions {

    public class ManagerInitialisationFailedException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerInitialisationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ManagerInitialisationFailedException(string message) : base(message) {
        }
    }
}