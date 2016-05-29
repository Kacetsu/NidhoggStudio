using System;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public class DoubleProperty : NumberProperty<double> {

        public DoubleProperty() : base() {
            Max = double.MaxValue;
            Min = double.MinValue;
        }

        public DoubleProperty(string name, double value) : base(name, value, double.MinValue, double.MaxValue) {
        }

        public DoubleProperty(string name, bool isOutput) : base(name, 0, double.MinValue, double.MaxValue) {
            IsOutput = isOutput;
        }

        public DoubleProperty(string name, double value, double min, double max) : base(name, value, min, max) {
        }

        public override Type Type {
            get {
                return typeof(double);
            }
        }
    }
}