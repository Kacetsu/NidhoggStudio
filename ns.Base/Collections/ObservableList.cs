using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ns.Base.Collections {

    [Serializable]
    public class ObservableList<T> : ObservableCollection<T> {

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        public ObservableList() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ObservableList(IEnumerable<T> collection) : base(collection) {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            Items.Clear();
            foreach (T obj in collection) Items.Add(obj);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableList{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ObservableList(ObservableList<T> collection) : base(collection) {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            Items.Clear();
            foreach (T obj in collection.Items) Items.Add(obj);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ICollection<T> FindAll(Predicate<T> match) {
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