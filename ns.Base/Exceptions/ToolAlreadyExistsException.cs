using System;

namespace ns.Base.Exceptions {

    public class ToolAlreadyExistsException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ToolAlreadyExistsException(string message) : base(message) {
        }
    }
}