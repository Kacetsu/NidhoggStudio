using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base {

    /// <summary>
    /// Base Class for all used Operations, Tools, Devices, Extensions and Properties.
    /// </summary>
    [Serializable, DataContract(IsReference = true), KnownType(typeof(Plugin)), KnownType(typeof(Tool)), KnownType(typeof(Property))]
    public class Node : NotifiableObject, ICloneable<Node>, INode {
        private string _fullname = string.Empty;
        private bool _isInitialized = false;
        private bool _isSelected = false;
        private string _name = string.Empty;
        private Node _parent = null;

        /// <summary>
        /// Base Class for all used Operations, Tools, Devices, Extensions and Properties.
        /// Creates the base fields: Name, Fullname, Childs, Cache and UID.
        /// </summary>
        public Node() {
            Items = new ObservableList<Node>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Node(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));

            UID = GenerateUID();
            Fullname = node.Fullname;
            Name = node.Name;

            Items = new ObservableList<Node>(node.Items.Select(item => item.Clone()).ToList());

            Parent = node.Parent;

            foreach (var child in Items) {
                child.Parent = this;
            }
        }

        /// <summary>
        /// Will be triggered if the Property did changed.
        /// </summary>
        /// <param name="sender">The object that this changed.</param>
        /// <param name="e">The Informations about the changed Property.</param>
        protected delegate void EventHandler<NodeChangedEventArgs>(object sender, NodeChangedEventArgs e);

        /// <summary>
        /// Gets or sets the Fullname.
        /// </summary>
        [DataMember]
        public string Fullname {
            get {
                if (string.IsNullOrEmpty(_fullname))
                    _fullname = ToString();
                return _fullname;
            }
            set {
                _fullname = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets if the Plugin is inizialized.
        /// </summary>
        /// <returns></returns>
        public bool IsInitialized { get { return _isInitialized; } }

        /// <summary>
        /// Gets or sets if the Node is selected;
        /// </summary>
        [DataMember]
        public bool IsSelected {
            get { return _isSelected; }
            set {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the list with all items.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [DataMember]
        public ObservableList<Node> Items { get; set; } = new ObservableList<Node>();

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [DataMember]
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name))
                    _name = GetType().ToString();
                return _name;
            }
            set {
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [XmlIgnore]
        public Node Parent {
            get { return _parent; }
            set {
                if (_parent != value) {
                    if (_parent != null) {
                        _parent.PropertyChanged -= ParentPropertyChangedEvent;
                    }
                    _parent = value;
                    if (_parent != null) {
                        _parent.PropertyChanged += ParentPropertyChangedEvent;
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of the tree.
        /// </summary>
        /// <value>
        /// The name of the tree.
        /// </value>
        public string TreeName {
            get {
                Node parent = Parent;
                string name = string.Empty;

                if (parent != null) {
                    name = parent.TreeName + "/" + Name;
                } else {
                    name = Name;
                }

                return name;
            }
        }

        /// <summary>
        /// Gets or sets the unified identification.
        /// </summary>
        [DataMember]
        public string UID { get; set; } = GenerateUID();

        /// <summary>
        /// Generates a new UID.
        /// </summary>
        /// <returns>The new UID.</returns>
        public static string GenerateUID() {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Adds a new Node to the Parent.
        /// Will trigger internal the OnChildCollectionChanged Method (ChildCollectionChanged).
        /// </summary>
        /// <param name="child">The Node that should be added.</param>
        public void AddChild(Node child) {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (Items.Contains(child) == false) {
                child.Parent = this;
                Items.Add(child);
            }
        }

        /// <summary>
        /// Adds a list of Childs to the Parent.
        /// Will trigger internal the OnChildCollectionChanged Method (ChildCollectionChanged).
        /// </summary>
        /// <param name="childs">The list of Childs that should be added.</param>
        public virtual void AddChilds(ICollection<Node> childs) {
            if (childs == null) throw new ArgumentNullException(nameof(childs));

            List<Node> addedChilds = new List<Node>();
            foreach (Node child in childs) {
                if (!Items.Contains(child)) {
                    child.Parent = this;
                    Items.Add(child);
                    addedChilds.Add(child);
                }
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>The cloned Node.</returns>
        public virtual Node Clone() => new Node(this);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() {
            _isInitialized = false;

            foreach (Node child in Items) {
                if (!child.IsInitialized) continue;
                child.Close();
            }
        }

        /// <summary>
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>Success of the Operation.</returns>
        public virtual bool Initialize() {
            _isInitialized = true;
            return true;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public virtual void RemoveChild(Node child) {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (Items.Contains(child)) {
                child.RemoveChilds();

                Items.Remove(child);
            }
        }

        /// <summary>
        /// Removes the childs.
        /// </summary>
        public virtual void RemoveChilds() {
            foreach (Property child in Items.Where(c => c is Property)) {
                child.Unconnect();
                child.RemoveChilds();
            }
            Items.Clear();
        }

        /// <summary>
        /// Parents the property changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ParentPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(Name)) || e.PropertyName.Equals(nameof(TreeName))) {
                OnPropertyChanged(e.PropertyName);
            }
        }
    }
}