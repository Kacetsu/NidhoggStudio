using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ns.Base.Collections {

    public class ObservableConcurrentDictionary<TKey, TValue> : ConcurrentDictionary<TKey, TValue> {

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey, TValue}"/> class.
        /// </summary>
        public ObservableConcurrentDictionary()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConcurrentDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> other)
            : base(other) {
        }

        /// <summary>
        /// Attempts to add the specified key and value to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be  null for reference types.</param>
        /// <returns>
        /// true if the key/value pair was added to the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> successfully; false if the key already exists.
        /// </returns>
        public new bool TryAdd(TKey key, TValue value) {
            if (base.TryAdd(key, value)) {
                return true;
            }

            return false;
        }
    }
}