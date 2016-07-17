using System;
using System.ComponentModel;

namespace ns.GUI.WPF.Controls.Property {

    public class NumberPropertyControl<T, U> : PropertyControl<T> where T : Base.Plugins.Properties.Property {
        private U _value;

        public NumberPropertyControl() : base() {
            DataContext = this;
            PropertyChanged += PropertyControl_PropertyChanged;
        }

        public NumberPropertyControl(T property) : base(property) {
            DataContext = this;
            PropertyChanged += PropertyControl_PropertyChanged;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public U Value {
            get { return _value; }
            set {
                if (_value.Equals(value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of the PropertyControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void PropertyControl_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            throw new NotImplementedException();
        }
    }
}