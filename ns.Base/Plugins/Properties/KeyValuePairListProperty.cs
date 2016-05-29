using System;
using System.Collections.Generic;

namespace ns.Base.Plugins.Properties {

    public class KeyValuePairListProperty : ListProperty {
        private List<KeyValuePair<string, object>> _keyValuePairs = new List<KeyValuePair<string, object>>();

        public KeyValuePairListProperty() : base() {
        }

        public KeyValuePairListProperty(string name, List<string> keys, List<object> values) : base(name, values) {
            for (int index = 0; index < keys.Count && index < values.Count; index++) {
                _keyValuePairs.Add(new KeyValuePair<string, object>(keys[index], values[index]));
            }
        }

        public override Type Type {
            get {
                return typeof(List<KeyValuePair<string, object>>);
            }
        }
    }
}