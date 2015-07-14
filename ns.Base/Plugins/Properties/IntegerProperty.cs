using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class IntegerProperty : NumberProperty {
        public IntegerProperty() : base() { }
        public IntegerProperty(string name, int value) : base(name, value, int.MinValue, int.MaxValue) { }
        public IntegerProperty(string name, bool isOutput) : base(name, 0, int.MinValue, int.MaxValue) { IsOutput = isOutput; }
        public IntegerProperty(string name, int value, int min, int max) : base(name, value, min, max) { }

        public override Type Type {
            get {
                return typeof(int);
            }
        }
    }
}
