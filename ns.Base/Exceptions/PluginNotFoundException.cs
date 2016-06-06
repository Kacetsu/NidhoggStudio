using System;

namespace ns.Base.Exceptions {

    public class PluginNotFoundException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PluginNotFoundException(string message) : base(message) {
        }
    }
}