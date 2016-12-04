using ns.Core;
using System;

namespace ns.Communication {

    public class CoreComHelper {

        /// <summary>
        /// Initializes the core system and communication manager.
        /// </summary>
        /// <exception cref="System.TypeInitializationException">
        /// null
        /// or
        /// null
        /// </exception>
        public static void InitializeCoreSystemAndCommunicationManager() {
            CoreSystem.Instance.TryInitialize();
            if (!CoreSystem.Instance.IsInitialized) {
                throw new TypeInitializationException(nameof(CoreSystem), null);
            }

            CommunicationManager.Instance.Connect();

            if (!CommunicationManager.Instance.IsConnected) {
                throw new TypeInitializationException(nameof(CommunicationManager), null);
            }
        }
    }
}