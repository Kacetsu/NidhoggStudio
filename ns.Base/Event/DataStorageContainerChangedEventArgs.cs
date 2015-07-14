using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class DataStorageContainerChangedEventArgs : EventArgs {
        private Property _property;
        private DataStorageContainer _container;

        public Property Property {
            get { return _property; }
        }

        public DataStorageContainer Container {
            get { return _container; }
        }

        public DataStorageContainerChangedEventArgs(Property property, DataStorageContainer container) {
            _property = property;
            _container = container;
        }
    }
}
