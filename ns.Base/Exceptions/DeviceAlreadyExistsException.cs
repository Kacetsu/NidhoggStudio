using System;

namespace ns.Base.Exceptions {

    public class DeviceAlreadyExistsException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceAlreadyExistsException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeviceAlreadyExistsException(string message) : base(message) {
        }
    }
}