using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Plugins {

    [Serializable]
    public class Tolerance<T> : INotifyPropertyChanged {
        private T _min;
        private T _max;

        public event PropertyChangedEventHandler PropertyChanged;

        public Tolerance() {
        }

        public Tolerance(T min, T max) {
            _min = min;
            _max = max;
        }

        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public T Min {
            get { return _min; }
            set {
                _min = value;
                OnPropertyChanged("Min");
            }
        }

        public T Max {
            get { return _max; }
            set {
                _max = value;
                OnPropertyChanged("Max");
            }
        }
    }
}