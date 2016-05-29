using System;

namespace ns.Base.Plugins.Properties {

    [Serializable]
    public class IntegerProperty : NumberProperty<int> {

        public IntegerProperty() : base() {
            Max = int.MaxValue;
            Min = int.MinValue;
        }

        public IntegerProperty(string name, int value) : base(name, value, int.MinValue, int.MaxValue) {
        }

        public IntegerProperty(string name, bool isOutput) : base(name, 0, int.MinValue, int.MaxValue) {
            IsOutput = isOutput;
        }

        public IntegerProperty(string name, int value, int min, int max) : base(name, value, min, max) {
        }
    }
}