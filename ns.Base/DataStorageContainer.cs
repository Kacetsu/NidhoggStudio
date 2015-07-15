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

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainer"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="treeName">Name of the tree.</param>
        /// <param name="uid">The uid.</param>
        public DataStorageContainer(string name, string treeName, string uid) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = new List<object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainer"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="treeName">Name of the tree.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="value">The value.</param>
        public DataStorageContainer(string name, string treeName, string uid, object value) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = new List<object>();
            Values.Add(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStorageContainer"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="treeName">Name of the tree.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="values">The values.</param>
        public DataStorageContainer(string name, string treeName, string uid, List<object> values) {
            Name = name;
            TreeName = treeName;
            UID = uid;
            Values = values;
        }
    }
}
