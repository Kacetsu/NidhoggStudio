using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ns.Plugin.WPF.Statistic {
    /// <summary>
    /// Interaction logic for StatisticView.xaml
    /// </summary>
    public partial class StatisticView : UserControl {

        private bool _dispatcherCompleted = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticView"/> class.
        /// </summary>
        public StatisticView() {
            InitializeComponent();
            this.Loaded += StatisticView_Loaded;
        }

        private void StatisticView_Loaded(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            DataStorageManager dataStorageManager = CoreSystem.Managers.Find(m => m.Name.Contains("DataStorageManager")) as DataStorageManager;
            dataStorageManager.ContainerAddedEvent += DataStorageManagerContainerAddedEvent;
            dataStorageManager.ContainerRemovedEvent += DataStorageManagerContainerRemovedEvent;
            dataStorageManager.ContainerChangedEvent += DataStorageManagerContainerChangedEvent;
        }

        private void DataStorageManagerContainerChangedEvent(object sender, Base.Event.DataStorageContainerChangedEventArgs e) {
            if (!_dispatcherCompleted || !e.Property.IsNumeric) return;
            this.Dispatcher.BeginInvoke(new Action(() => {
                _dispatcherCompleted = false;
                foreach (TabItem item in this.Control.Items) {
                    if (item is StatisticPage) {
                        StatisticPage page = item as StatisticPage;
                        if (page.Property == e.Property) {
                            // Update Container
                            page.UpdateContainer(e.Container);
                        }
                    }
                }
                _dispatcherCompleted = true;
            }));
        }

        private void DataStorageManagerContainerAddedEvent(object sender, Base.Event.DataStorageContainerChangedEventArgs e) {
            if (!e.Property.IsNumeric) return;
            this.Dispatcher.BeginInvoke(new Action(() => {
                bool contains = false;
                foreach (TabItem item in this.Control.Items) {
                    if (item is StatisticPage) {
                        StatisticPage page = item as StatisticPage;
                        if (page.Property == e.Property)
                            contains = true;
                    }
                }

                Property property = e.Property;

                if (property != null) {
                    if (!contains) {
                        this.Control.Items.Add(new StatisticPage(e.Property, e.Container));
                        if (this.Control.Items.Count == 1)
                            this.Control.SelectedIndex = 0;
                    }
                }
            }));
        }

        private void DataStorageManagerContainerRemovedEvent(object sender, Base.Event.DataStorageContainerChangedEventArgs e) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                StatisticPage targetPage = null;
                foreach (TabItem item in this.Control.Items) {
                    if (item is StatisticPage) {
                        StatisticPage page = item as StatisticPage;
                        if (page.Property == e.Property)
                            targetPage = page;
                    }
                }

                if(targetPage != null)
                    this.Control.Items.Remove(targetPage);

            }));
        }
    }
}
