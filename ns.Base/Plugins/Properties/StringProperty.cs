using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class StringProperty : Property {
        public StringProperty() : base() { }
        public StringProperty(string name, string value) : base(name, value) { }
        public StringProperty(string name, bool isOutput) : base(name, isOutput) { }

        public override Type Type {
            get {
                return typeof(string);
            }
        }
    }
}
