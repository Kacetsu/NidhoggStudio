using System;

namespace ns.Base.Exceptions {

    [Serializable]
    public class OperationAlreadyExistsException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OperationAlreadyExistsException(string message) : base(message) { }
    }
}