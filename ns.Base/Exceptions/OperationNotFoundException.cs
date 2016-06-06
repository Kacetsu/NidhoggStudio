using System;

namespace ns.Base.Exceptions {

    public class OperationNotFoundException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OperationNotFoundException(string message) : base(message) {
        }
    }
}