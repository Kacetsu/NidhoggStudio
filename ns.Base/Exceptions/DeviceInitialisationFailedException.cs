using System;

namespace ns.Base.Exceptions {

    public class DeviceInitialisationFailedException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInitialisationFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeviceInitialisationFailedException(string message) : base(message) {
        }
    }
}