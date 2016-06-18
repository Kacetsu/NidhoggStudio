using ns.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Communication.CommunicationModels {

    [DataContract]
    public class GenericCommunicationModel<T> : IGenericCommunicationModel<T>, INotifyPropertyChanged where T : Node {

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Fullname { get; private set; }

        [DataMember]
        public bool IsSelected { get; set; }

        [DataMember]
        public string UID { get; private set; }

        public GenericCommunicationModel() {
        }

        public GenericCommunicationModel(T node) : this() {
            Name = node.Name;
            Fullname = node.Fullname;
            IsSelected = node.IsSelected;
            UID = node.UID;
        }

        /// <summary>
        /// Factories the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static GenericCommunicationModel<T> Factory(T node) {
            return new GenericCommunicationModel<T>(node);
        }
    }
}