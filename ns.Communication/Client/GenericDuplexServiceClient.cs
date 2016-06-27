using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ns.Communication.Client {

    public class GenericDuplexServiceClient<T, U> : IDisposable where T : class where U : class {
        private bool disposed = false;

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        protected T Channel { get; private set; }

        /// <summary>
        /// Gets the callback.
        /// </summary>
        /// <value>
        /// The callback.
        /// </value>
        public U Callback { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDuplexServiceClient{T}" /> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="callback">The callback.</param>
        /// <exception cref="System.InvalidCastException"></exception>
        protected GenericDuplexServiceClient(EndpointAddress endpoint, Binding binding, U callback) {
            Channel = DuplexChannelFactory<T>.CreateChannel(callback, binding, endpoint);
            Callback = callback;
            ICommunicationObject comObject = (Channel as ICommunicationObject);
            if (comObject == null) throw new InvalidCastException();

            comObject.Faulted += Handle_Faulted;
            comObject.Open();
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            (Channel as ICommunicationObject)?.Close();
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

        private void Handle_Faulted(object sender, EventArgs e) {
        }
    }
}