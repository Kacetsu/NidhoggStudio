using ns.Base;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ns.Communication.Models {

    [DataContract]
    public abstract class GenericModel<T> : IGenericModel<T>, INotifyPropertyChanged where T : Node {

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericModel{T}"/> class.
        /// </summary>
        public GenericModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericModel{T}"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public GenericModel(T node) : this() {
            Name = node.Name;
            Fullname = node.Fullname;
            IsSelected = node.IsSelected;
            Id = node.Id;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the fullname.
        /// </summary>
        /// <value>
        /// The fullname.
        /// </value>
        [DataMember]
        public string Fullname { get; private set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public Guid Id { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}