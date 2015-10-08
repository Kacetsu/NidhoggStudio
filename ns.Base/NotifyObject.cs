using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base {
    [Serializable]
    public class NotifyObject : INotifyPropertyChanged {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
