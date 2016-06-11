using System;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class StringProperty : GenericProperty<string> {

        public StringProperty() : base() {
        }

        public StringProperty(string name, string value) : base(name, value) {
        }

        public StringProperty(string name, bool isOutput) : base(name, isOutput) {
        }
    }
}