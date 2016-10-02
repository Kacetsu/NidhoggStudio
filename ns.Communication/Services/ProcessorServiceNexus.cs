using ns.Base.Log;
using ns.Communication.Models;
using ns.Communication.Services.Callbacks;
using ns.Core;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ns.Communication.Services {

    internal class ProcessorServiceNexus : Nexus<IProcessorServiceCallbacks> {

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <returns></returns>
        public static ProcessorInfoModel GetState() => new ProcessorInfoModel(CoreSystem.Processor);

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public static void Start() {
            if (!CoreSystem.Processor.Start()) {
                throw new FaultException(string.Format("Could not start processor!"));
            } else {
                List<Guid> damagedIds = new List<Guid>();
                Parallel.ForEach(_clients, (client) => {
                    try {
                        client.Value?.OnProcessorStarted(new ProcessorInfoModel(CoreSystem.Processor));
                    } catch (CommunicationException ex) {
                        Trace.WriteLine(ex, System.Diagnostics.TraceEventType.Warning);
                        damagedIds.Add(client.Key);
                    }
                });

                RemoveDisconnectedClients(damagedIds);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <exception cref="FaultException"></exception>
        public static void Stop() {
            if (!CoreSystem.Processor.Stop()) {
                throw new FaultException(string.Format("Could not stop processor!"));
            } else {
                foreach (var client in _clients) {
                    client.Value?.OnProcessorStopped(new ProcessorInfoModel(CoreSystem.Processor));
                }
            }
        }
    }
}