using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Controls {

    /// <summary>
    /// Logic for <see cref="ResultsView"/>.
    /// </summary>
    public partial class ResultsView : UserControl {
        private LockedObservableCollection<ResultViewContainer> _collection = new LockedObservableCollection<ResultViewContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsView"/> class.
        /// </summary>
        public ResultsView() {
            InitializeComponent();
            ContenList.ItemsSource = _collection;
            Loaded += ResultsView_Loaded;
        }

        /// <summary>
        /// Gets the tab button.
        /// </summary>
        /// <value>
        /// The tab button.
        /// </value>
        public Button TabButton => _tabButton;

        private void FrontendManager_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(FrontendManager.SelectedPluginProperties))) {
                Dispatcher.Invoke(new System.Action(() => {
                    Base.Plugins.Properties.Property property = FrontendManager.SelectedPluginProperties.First();
                    if (_collection.Count == 0 || _collection.FirstOrDefault(r => r.Property?.Id.Equals(property.Id) == true) == null) {
                        _collection.Clear();
                        foreach (Base.Plugins.Properties.Property prop in FrontendManager.SelectedPluginProperties.Where(p => p.Direction == Base.Plugins.Properties.PropertyDirection.Out)) {
                            _collection.Add(new ResultViewContainer(prop));
                        }
                    } else {
                        foreach (Base.Plugins.Properties.Property prop in FrontendManager.SelectedPluginProperties.Where(p => p.Direction == Base.Plugins.Properties.PropertyDirection.Out)) {
                            ResultViewContainer container = _collection.FirstOrDefault(r => r.Property?.Id.Equals(prop.Id) == true);
                            if (container == null) continue;
                            container.UpdateProperty(prop);
                        }
                    }
                }));
            }
        }

        private void ResultsView_Loaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            FrontendManager.Instance.PropertyChanged += FrontendManager_PropertyChanged;
        }
    }
}