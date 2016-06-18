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
        private bool _isInitialized = false;

        private bool _isSelected = false;

        /// <summary>
        /// Will be triggered if the Property did changed.
        /// </summary>
        /// <param name="sender">The object that this changed.</param>
        /// <param name="e">The Informations about the changed Property.</param>
        public delegate void NodeChangedEventHandler(object sender, NodeChangedEventArgs e);

        protected string _name = string.Empty;

        private string _fullname = string.Empty;

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
        }

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
        /// Gets or sets the unified identification.
        /// </summary>
        [DataMember]
        public string UID { get; set; } = GenerateUID();

        /// <summary>
        /// Gets or sets the list with all Childs.
        /// </summary>
        [DataMember]
        public ObservableList<Node> Childs { get; set; } = new ObservableList<Node>();

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [XmlIgnore]
        public Node Parent { get; private set; }

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
        /// Initialze the Plugin.
        /// </summary>
        /// <returns>Success of the Operation.</returns>
        public virtual bool Initialize() {
            _isInitialized = true;
            return true;
        }

        /// <summary>
        /// Finalize the Node.
        /// </summary>
        /// <returns>Success of the Operation.</returns>
        public virtual bool Finalize() {
            _isInitialized = false;

            bool result = true;

            foreach (Node child in Childs) {
                if (!child.IsInitialized) continue;
                if (!child.Finalize())
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// Adds a new Node to the Parent.
        /// Will trigger internal the OnChildCollectionChanged Method (ChildCollectionChanged).
        /// </summary>
        /// <param name="child">The Node that should be added.</param>
        public virtual void AddChild(Node child) {
            lock (Childs) {
                if (Childs.Contains(child) == false) {
                    child.SetParent(this);
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
                    child.SetParent(this);
                    Childs.Add(child);
                    addedChilds.Add(child);
                }
            }
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
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void SetParent(Node parent) {
            if (Parent != null)
                Parent.PropertyChanged -= ParentPropertyChangedEvent;
            Parent = parent;
            Parent.PropertyChanged += ParentPropertyChangedEvent;
        }

        /// <summary>
        /// Parents the property changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ParentPropertyChangedEvent(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name" || e.PropertyName == "TreeName") {
                OnPropertyChanged(e.PropertyName);
            }
        }

        /// <summary>
        /// Generates a new UID.
        /// </summary>
        /// <returns>The new UID.</returns>
        public static string GenerateUID() {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// Will set a new UID.
        /// </summary>
        /// <returns>The cloned Node.</returns>
        public virtual object Clone() {
            Node nodeClone = (Node)MemberwiseClone();
            nodeClone.UID = GenerateUID();
            nodeClone.Name = Name;
            nodeClone.Fullname = Fullname;
            return nodeClone;
        }
    }
}