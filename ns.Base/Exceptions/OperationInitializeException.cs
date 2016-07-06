using System;

namespace ns.Base.Exceptions {

    public class OperationInitializeException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationInitializeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OperationInitializeException(string message) : base(message) {
        }
    }
}