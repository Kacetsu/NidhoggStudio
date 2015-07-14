using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl {
        private LockedObservableCollection<ns.Base.Plugins.Properties.Property> _collection = new LockedObservableCollection<ns.Base.Plugins.Properties.Property>();

        private PropertyManager _propertyManager;
        private GuiManager _guiManager;

        public ResultsView() {
            InitializeComponent();
            this.Loaded += ResultsView_Loaded;
        }

        private void ResultsView_Loaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataControl.ItemsSource = _collection;
            _propertyManager = CoreSystem.Managers.Find(m => m.Name.Contains("PropertyManager")) as PropertyManager;
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
            _propertyManager.NodeAddedEvent += PropertyManagerNodeAddedEvent;
            _propertyManager.NodeRemovedEvent += PropertyManagerNodeRemovedEvent;
            projectManager.Loading += ProjectManagerLoading;
            _guiManager.SelectedItemChanged += guiManager_SelectedItemChanged;
        }

        private void guiManager_SelectedItemChanged(object sender, Base.Event.NodeSelectionChangedEventArgs e) {
            if (e.SelectedNode == null) return;
            List<object> properties = e.SelectedNode.Childs.FindAll(c => c is ns.Base.Plugins.Properties.Property);
            _collection.Clear();
            foreach (ns.Base.Plugins.Properties.Property property in properties) {
                if (property.IsOutput && !_collection.Contains(property) && property.Parent is Tool) {
                    _collection.Add(property);
                }
            }
        }

        private void ProjectManagerLoading() {
            _collection.Clear();
        }

        private void PropertyManagerNodeAddedEvent(object sender, Base.Event.NodeCollectionChangedEventArgs e) {
            if (e.Node is ns.Base.Plugins.Properties.Property) {
                ns.Base.Plugins.Properties.Property property = e.Node as ns.Base.Plugins.Properties.Property;

                if (property.Parent != _guiManager.SelectedNode) return;

                if (property.IsOutput && !_collection.Contains(property) && property.Parent is Tool) {
                    _collection.Add(property);
                }
            }
        }

        private void PropertyManagerNodeRemovedEvent(object sender, Base.Event.NodeCollectionChangedEventArgs e) {
            if (e.Node is ns.Base.Plugins.Properties.Property) {
                ns.Base.Plugins.Properties.Property property = e.Node as ns.Base.Plugins.Properties.Property;

                if (property.Parent != _guiManager.SelectedNode) return;

                if (_collection.Contains(property))
                    _collection.Remove(property);
            }
        }
    }
}
