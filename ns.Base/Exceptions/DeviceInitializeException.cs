using System;

namespace ns.Base.Exceptions {

    [Serializable]
    public class DeviceInitializeException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInitializeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeviceInitializeException(string message) : base(message) {
        }
    }
}