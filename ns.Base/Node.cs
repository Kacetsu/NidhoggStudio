using ns.Base.Collections;
using ns.Base.Communication;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ns.Base {

    /// <summary>
    /// Base Class for all used Operations, Tools, Devices, Extensions and Properties.
    /// </summary>
    [DataContract(IsReference = true)]
    [KnownType(typeof(Plugin))]
    [KnownType(typeof(Property))]
    public abstract class Node : NotifiableObject, ICloneable<Node>, INode {
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public Node(Node node) {
            if (node == null) throw new ArgumentNullException(nameof(node));

            Id = GenerateId();
            Fullname = node.Fullname;
            Name = node.Name;

            Items = new ObservableConcurrentDictionary<Guid, Node>();
            foreach (KeyValuePair<Guid, Node> pair in node.Items) {
                Node nodeCopy = pair.Value.Clone();
                Items.TryAdd(nodeCopy.Id, nodeCopy);
            }

            Parent = node.Parent;

            foreach (var child in Items) {
                child.Value.Parent = this;
            }
        }

        /// <summary>
        /// Will be triggered if the Property did changed.
        /// </summary>
        /// <param name="sender">The object that this changed.</param>
        /// <param name="e">The Informations about the changed Property.</param>
        protected delegate void EventHandler<NodeChangedEventArgs>(object sender, NodeChangedEventArgs e);

        /// <summary>
        /// Gets the communication model.
        /// </summary>
        /// <value>
        /// The communication model.
        /// </value>
        public virtual CommunicationModel CommunicationModel => new NodeCommunicationModel(this);

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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [DataMember]
        public Guid Id { get; set; } = GenerateId();

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
        /// Gets or sets the dictionary with all items.
        /// </summary>
        [DataMember]
        public ObservableConcurrentDictionary<Guid, Node> Items { get; set; } = new ObservableConcurrentDictionary<Guid, Node>();

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
                    name = string.Concat(parent.TreeName, "/", Name);
                } else {
                    name = Name;
                }

                return name;
            }
        }

        /// <summary>
        /// Adds a new Node to the Parent.
        /// Will trigger internal the OnChildCollectionChanged Method (ChildCollectionChanged).
        /// </summary>
        /// <param name="child">The Node that should be added.</param>
        public void AddChild(Node child) {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (!Items.ContainsKey(child.Id)) {
                child.Parent = this;
                Items.TryAdd(child.Id, child);
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
                if (!Items.ContainsKey(child.Id)) {
                    child.Parent = this;
                    Items.TryAdd(child.Id, child);
                    addedChilds.Add(child);
                }
            }
        }

        /// <summary>
        /// Clones the Node with all its Members.
        /// </summary>
        /// <returns>The cloned Node.</returns>
        public virtual Node Clone() { throw new NotImplementedException(); }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public virtual void Close() {
            _isInitialized = false;

            foreach (Node child in Items.Values.Where(c => c.IsInitialized)) {
                child.Close();
            }
        }

        /// <summary>
        /// Finds the or add.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TType FindOrAdd<TType>([CallerMemberName] string name = null) where TType : Node {
            TType node = Items.FirstOrDefault(i => string.Equals(i.Value.Name, name, StringComparison.Ordinal)).Value as TType;
            if (node == null) {
                node = Activator.CreateInstance(typeof(TType), name) as TType;
                Items.TryAdd(node.Id, node);
            }

            return node;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize() {
            foreach (var property in GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty)) {
                var value = property.GetValue(this);
                Node node = value as Node;
                if (node != null) {
                    node.Initialize();
                }
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">The child.</param>
        public virtual void RemoveChild(Node child) {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (Items.ContainsKey(child.Id)) {
                child.RemoveChilds();
                Node outNode;
                Items.TryRemove(child.Id, out outNode);
            }
        }

        /// <summary>
        /// Removes the childs.
        /// </summary>
        public virtual void RemoveChilds() {
            foreach (Property child in Items.Values.OfType<Property>()) {
                child.Unconnect();
                child.RemoveChilds();
            }
            Items.Clear();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
        public bool TryInitialize() {
            try {
                Initialize();
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// Generates a new UID.
        /// </summary>
        /// <returns>The new UID.</returns>
        private static Guid GenerateId() => Guid.NewGuid();

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