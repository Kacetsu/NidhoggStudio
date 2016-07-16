using ns.Base.Event;
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
    [DataContract(IsReference = true), KnownType(typeof(Plugin)), KnownType(typeof(Tool)), KnownType(typeof(Property))]
    public class Node : NotifiableObject, ICloneable, INode {
        protected string _name = string.Empty;
        private string _fullname = string.Empty;
        private bool _isInitialized = false;

        private bool _isSelected = false;

        private Node _parent = null;

        /// <summary>
        /// Base Class for all used Operations, Tools, Devices, Extensions and Properties.
        /// Creates the base fields: Name, Fullname, Childs, Cache and UID.
        /// </summary>
        public Node() {
            Childs = new ObservableList<Node>();
            UID = GenerateUID();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Node(Node node) {
            UID = node.UID;
            Fullname = node.Fullname;
            Name = node.Name;
            Childs = new ObservableList<Node>(node.Childs);
            Parent = node.Parent;

            foreach (var child in Childs) {
                child.Parent = this;
            }
        }

        /// <summary>
        /// Will be triggered if the Property did changed.
        /// </summary>
        /// <param name="sender">The object that this changed.</param>
        /// <param name="e">The Informations about the changed Property.</param>
        public delegate void NodeChangedEventHandler(object sender, NodeChangedEventArgs e);

        /// <summary>
        /// Gets or sets the list with all Childs.
        /// </summary>
        [DataMember]
        public ObservableList<Node> Childs { get; set; } = new ObservableList<Node>();

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
        /// Gets or sets the Name.
        /// </summary>
        [DataMember]
        public virtual string Name {
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
                    _parent.PropertyChanged += ParentPropertyChangedEvent;
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
        public virtual void AddChild(Node child) {
            lock (Childs) {
                if (Childs.Contains(child) == false) {
                    child.Parent = this;
                    Childs.Add(child);
                }
            }
        }

        /// <summary>
        /// Adds a list of Childs to the Parent.
        /// Will trigger internal the OnChildCollectionChanged Method (ChildCollectionChanged).
        /// </summary>
        /// <param name="childs">The list of Childs that should be added.</param>
        public virtual void AddChilds(List<Node> childs) {
            List<Node> addedChilds = new List<Node>();
            foreach (Node child in childs) {
                if (!Childs.Contains(child)) {
                    child.Parent = this;
                    Childs.Add(child);
                    addedChilds.Add(child);
                }
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>The cloned Node.</returns>
        public virtual object Clone() => new Node(this);

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() {
            _isInitialized = false;

            foreach (Node child in Childs) {
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
            lock (Childs) {
                if (Childs.Contains(child)) {
                    child.RemoveChilds();
                    Childs.Remove(child);
                }
            }
        }

        /// <summary>
        /// Removes the childs.
        /// </summary>
        public virtual void RemoveChilds() {
            foreach (Property child in Childs.Where(c => c is Property)) {
                child.Unconnect();
                child.RemoveChilds();
            }
            Childs.Clear();
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