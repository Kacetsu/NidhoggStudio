using ns.Communication.Services;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class GenericDuplexServiceClient<T> : IDisposable where T : class {
        private bool disposed = false;

        protected T Channel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDuplexServiceClient{T}"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        protected GenericDuplexServiceClient(EndpointAddress endpoint, Binding binding, object callback) {
            Channel = DuplexChannelFactory<T>.CreateChannel(callback, binding, endpoint);
            ICommunicationObject comObject = (Channel as ICommunicationObject);
            if (comObject == null) throw new InvalidCastException();
            comObject.Open();
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() => (Channel as ICommunicationObject)?.Close();

        protected virtual void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                if (Channel != null) {
                    Close();
                    Channel = null;
                    disposed = true;
                }
            }
        }
    }
}