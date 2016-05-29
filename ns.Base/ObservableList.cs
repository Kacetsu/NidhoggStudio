using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ns.Base {

    [Serializable]
    public class ObservableList<T> : ObservableCollection<T> {

        public ObservableList() : base() {
        }

        public ObservableList(IEnumerable<T> collection) : base(collection) {
            Items.Clear();
            foreach (T obj in collection) Items.Add(obj);
        }

        public ObservableList(ObservableList<T> collection) : base(collection) {
            Items.Clear();
            foreach (T obj in collection.Items) Items.Add(obj);
        }

        public T Find(Predicate<T> match) {
            if (match == null) {
                throw new ArgumentNullException(nameof(match));
            }
            for (int i = 0; i < Count; i++) {
                if (match(Items[i])) {
                    return Items[i];
                }
            }
            return default(T);
        }

        public List<T> FindAll(Predicate<T> match) {
            if (match == null) {
                throw new ArgumentNullException(nameof(match));
            }
            List<T> list = new List<T>();
            for (int i = 0; i < Count; i++) {
                if (match(Items[i])) {
                    list.Add(Items[i]);
                }
            }
            return list;
        }
    }
}