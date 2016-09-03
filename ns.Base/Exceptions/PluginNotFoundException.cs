using System;
using System.Runtime.Serialization;

namespace ns.Base.Exceptions {

    [Serializable]
    public class PluginNotFoundException : Exception {

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PluginNotFoundException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Die Fehlermeldung, in der die Ursache der Ausnahme erklärt wird.</param>
        /// <param name="innerException">Die Ausnahme, die die aktuelle Ausnahme verursacht hat, oder ein Nullverweis (Nothing in Visual Basic), wenn keine innere Ausnahme angegeben ist.</param>
        public PluginNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
        /// </summary>
        public PluginNotFoundException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginNotFoundException"/> class.
        /// </summary>
        /// <param name="info">Die <see cref="T:System.Runtime.Serialization.SerializationInfo" />, die die serialisierten Objektdaten für die ausgelöste Ausnahme enthält.</param>
        /// <param name="context">Der <see cref="T:System.Runtime.Serialization.StreamingContext" />, der die Kontextinformationen über die Quelle oder das Ziel enthält.</param>
        protected PluginNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}