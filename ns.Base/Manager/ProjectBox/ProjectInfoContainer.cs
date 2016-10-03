using ns.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Manager.ProjectBox {

    [DataContract]
    public class ProjectInfoContainer : INotifyPropertyChanged, IIdentifiable {
        private bool _isUsed = false;
        private string _name = string.Empty;
        private string _path = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInfoContainer"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="name">The name.</param>
        public ProjectInfoContainer(string path, string name) {
            _path = path;
            _name = name;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is used.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsUsed {
            get { return _isUsed; }
            set {
                if (_isUsed == value) return;
                _isUsed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name {
            get { return _name; }
            set {
                if (_name?.Equals(value) == true) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path {
            get { return _path; }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="name">The name.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}