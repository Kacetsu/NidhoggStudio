using ns.Base.Log;
using ns.Base.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ns.Communication {

    public class CommunicationManager : IManager {
        private Task _serviceTask;
        private SemaphoreSlim _serviceStopSignal = new SemaphoreSlim(0, 1);

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name => GetType().Name;

        public string Address => "http://localhost:8080/";

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public bool Initialize() {
            try {
                _serviceTask = new Task(StartService);
                _serviceTask.Start();
            } catch (Exception ex) when (ex is ArgumentNullException || ex is ObjectDisposedException || ex is InvalidOperationException) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Finalizes this instance.
        /// </summary>
        /// <returns></returns>
        public bool Finalize() {
            try {
                _serviceStopSignal.Release();
                _serviceStopSignal.Wait();
            } catch (Exception ex) when (ex is ObjectDisposedException || ex is SemaphoreFullException) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Error);
                return false;
            }
            return true;
        }

        private void StartService() {
            Uri baseAddress = new Uri(Address);

            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(CommunicationService), baseAddress)) {
                host.AddServiceEndpoint(typeof(ICommunicationService), new WSDualHttpBinding(), baseAddress);
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                _serviceStopSignal.Wait();

                // Close the ServiceHost.
                host.Close();
            }

            _serviceStopSignal = new SemaphoreSlim(0, 1);
        }
    }
}