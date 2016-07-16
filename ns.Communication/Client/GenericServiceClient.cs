using ns.Base.Log;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class GenericServiceClient<T> : IDisposable where T : class {
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericServiceClient{T}"/> class.
        /// </summary>
        public GenericServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericServiceClient{T}"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        protected GenericServiceClient(EndpointAddress endpoint, Binding binding) {
            Channel = ChannelFactory<T>.CreateChannel(binding, endpoint);
            (Channel as ICommunicationObject)?.Open();
        }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public T Channel { get; protected set; }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            try {
                (Channel as ICommunicationObject)?.Close();
            } catch (CommunicationException ex) {
                Trace.WriteLine(ex.Message, System.Diagnostics.TraceEventType.Warning);
            }
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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