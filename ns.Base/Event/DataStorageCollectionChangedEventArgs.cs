using System;
using System.Collections.Generic;

namespace ns.Base.Event {

    public class DataStorageCollectionChangedEventArgs : EventArgs {

        public DataStorageCollectionChangedEventArgs(string containerUID, string pluginUID) : base() {
            NewContainers = new Dictionary<string, string>();
            NewContainers.Add(containerUID, pluginUID);
        }

        public DataStorageCollectionChangedEventArgs(Dictionary<string, string> containerDictionary) : base() {
            NewContainers = containerDictionary;
        }

        public Dictionary<string, string> NewContainers { get; private set; }
    }
}