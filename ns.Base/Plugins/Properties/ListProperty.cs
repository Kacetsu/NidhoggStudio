using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ns.Base.Plugins.Properties {

    [Serializable, DataContract]
    public class ListProperty : GenericProperty<List<object>>, IListProperty<object> {
        private List<object> _list = new List<object>();

        private object _selectedItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        public ListProperty() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="values">The values.</param>
        public ListProperty(string name, List<object> values) : base(name, values) {
            _list = values;
            if (values.Count == 0)
                SelectedItem = "INVALID";
            else
                SelectedItem = values[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="isOutput">True if the property is a output.</param>
        public ListProperty(string name, bool isOutput)
            : base(name, isOutput) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListProperty"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public ListProperty(ListProperty other) : base(other) {
            SelectedItem = other.SelectedItem;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index {
            get {
                if (Value == null) return -1;
                int index = 0;
                for (; index < _list.Count; index++) {
                    if (_list[index].ToString() == SelectedItem?.ToString()) break;
                    else if (_list[index] == SelectedItem) break;
                }
                return index;
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>
        /// The selected item.
        /// </value>
        [DataMember]
        public object SelectedItem {
            get { return _selectedItem; }
            set {
                if (_selectedItem != value) {
                    _selectedItem = value;
                    SelectedObjItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// Gets or sets the selected object item.
        /// </summary>
        /// <value>
        /// The selected object item.
        /// </value>
        public object SelectedObjItem {
            get { return SelectedItem; }
            set {
                if (SelectedItem != value) {
                    SelectedItem = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the selected type.
        /// </summary>
        /// <value>
        /// The name of the selected type.
        /// </value>
        [DataMember]
        public override Type Type {
            get {
                return typeof(List<object>);
            }
        }
    }
}