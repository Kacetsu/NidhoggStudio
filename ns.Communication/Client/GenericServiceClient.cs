using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.Client {

    public class GenericServiceClient<T> : IDisposable where T : class {
        private bool disposed = false;

        protected T Channel { get; private set; }

        protected GenericServiceClient(EndpointAddress endpoint, Binding binding) {
            Channel = ChannelFactory<T>.CreateChannel(binding, endpoint);
            (Channel as ICommunicationObject)?.Open();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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