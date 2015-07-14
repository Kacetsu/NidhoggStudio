using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base {
    public struct DataStorageContainer {
        public string Name;
        public string TreeName;
        public string UID;
        public List<object> Values;

        public DataStorageContainer(string name, string treeName, string uid) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = new List<object>();
        }

        public DataStorageContainer(string name, string treeName, string uid, object value) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = new List<object>();
            Values.Add(value);
        }

        public DataStorageContainer(string name, string treeName, string uid, List<object> values) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = values;
        }
    }
}
