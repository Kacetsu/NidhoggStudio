using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins.Properties {
    [Serializable]
    public class DoubleProperty : NumberProperty {
        public DoubleProperty() : base() {
            Max = Double.MaxValue;
            Min = Double.MinValue;
        }
        public DoubleProperty(string name, double value) : base(name, value, Double.MinValue, Double.MaxValue) { }
        public DoubleProperty(string name, bool isOutput) : base(name, 0, Double.MinValue, Double.MaxValue) { IsOutput = isOutput; }
        public DoubleProperty(string name, double value, double min, double max) : base(name, value, min, max) { }

        public override Type Type {
            get {
                return typeof(double);
            }
        }
    }
}
