using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ns.Base.Plugins {

    [Serializable, DataContract]
    public class Tolerance<T> : INotifyPropertyChanged {
        private T _min;
        private T _max;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tolerance{T}"/> class.
        /// </summary>
        public Tolerance() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tolerance{T}"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public Tolerance(T min, T max) {
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        private void OnPropertyChanged(string name) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        [DataMember]
        public T Min {
            get { return _min; }
            set {
                _min = value;
                OnPropertyChanged("Min");
            }
        }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        [DataMember]
        public T Max {
            get { return _max; }
            set {
                _max = value;
                OnPropertyChanged("Max");
            }
        }
    }
}