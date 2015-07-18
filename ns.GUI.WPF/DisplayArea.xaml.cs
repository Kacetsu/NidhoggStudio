using ns.Base.Event;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using ns.GUI.WPF.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ns.GUI.WPF {
    /// <summary>
    /// Interaction logic for DisplayArea.xaml
    /// </summary>
    public partial class DisplayArea : UserControl {
        private DisplayManager _displayManager;
        private bool _isDispatcherInactive = true;
        private Mutex _mutex = new Mutex();
        private GuiManager _guiManager;


        public DisplayArea() {
            InitializeComponent();
            this.Dispatcher.Hooks.DispatcherInactive += Hooks_DispatcherInactive;
        }

        private void LoadedHandle(object sender, RoutedEventArgs e) {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _displayManager = CoreSystem.Managers.Find(m => m.Name.Contains("DisplayManager")) as DisplayManager;
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
            foreach (Operation operation in _displayManager.Nodes) {
                AddTabParent(operation);
            }
            if (this.DisplayTabControl.Items.Count > 0)
                this.DisplayTabControl.SelectedIndex = 0;
            _displayManager.NodeAddedEvent += NodeAddedEvent;
            _displayManager.ImageChangedEvent += ImageChangedEvent;
            _displayManager.NodeRemovedEvent += DisplayManagerNodeRemovedEvent;
            _displayManager.ClearEvent += ClearEvent;
            _guiManager.SelectedItemChanged += GuiManagerSelectedItemChanged;
        }

        private void GuiManagerSelectedItemChanged(object sender, NodeSelectionChangedEventArgs e) {
            if (e.SelectedNode is Operation)
                return;
            else if (e.SelectedNode is Tool) {
                Operation operationParent = ((Tool)e.SelectedNode).Parent as Operation;
                if (operationParent == null)
                    return;
                foreach (OperationDisplayTabItem oItem in this.DisplayTabControl.Items) {
                    if (oItem.Operation == operationParent) {
                        foreach (DisplayTabItem dItem in ((TabControl)oItem.Content).Items) {
                            if (dItem.ImageProperty.ParentTool == e.SelectedNode) {
                                ((TabControl)oItem.Content).SelectedItem = dItem;
                            }
                        }
                    }
                }
            }
        }

        private void DisplayManagerNodeRemovedEvent(object sender, NodeCollectionChangedEventArgs e) {
            if (e.Node is Operation)
                RemoveTabParent(e.Node as Operation);
            else if (e.Node is ImageProperty)
                RemoveTabItem(e.Node as ImageProperty);
        }

        private void ClearEvent(object sender, EventArgs e) {
            this.DisplayTabControl.Items.Clear();
        }

        private void AddTabItem(ImageProperty image) {
            DisplayTabItem item = new DisplayTabItem(image);

            Operation parentOperation = image.ParentOperation as Operation;
            foreach (OperationDisplayTabItem oitem in this.DisplayTabControl.Items) {
                if (oitem.Operation == parentOperation) {
                    TabControl control = oitem.Content as TabControl;
                    control.Items.Add(item);
                    item.SetParent(control);
                }
            }
        }

        private void RemoveTabItem(ImageProperty image) {
            DisplayTabItem target = null;
            Operation parentOperation = image.ParentOperation as Operation;
            foreach (OperationDisplayTabItem item in this.DisplayTabControl.Items) {
                if (item.Operation == parentOperation) {

                    foreach (DisplayTabItem child in ((TabControl)item.Content).Items) {
                        if (child.ImageProperty == image) {
                            target = child;
                            break;
                        }
                    }

                    ((TabControl)item.Content).Items.Remove(target);

                    break;
                }
            }
        }

        private void AddTabParent(Operation operation) {
            OperationDisplayTabItem item = new OperationDisplayTabItem(operation);
            this.DisplayTabControl.Items.Add(item);
        }

        private void RemoveTabParent(Operation operation) {
            OperationDisplayTabItem target = null;
            foreach (OperationDisplayTabItem item in this.DisplayTabControl.Items) {
                if (item.Operation == operation) {
                    target = item;
                    break;
                }
            }
            this.DisplayTabControl.Items.Remove(target);
        }

        private void NodeAddedEvent(object sender, NodeCollectionChangedEventArgs e) {
            if(e.Node is Operation)
                AddTabParent(e.Node as Operation);

            if (e.Node is ImageProperty)
                AddTabItem(e.Node as ImageProperty);

            if (this.DisplayTabControl.Items.Count == 1) 
                this.DisplayTabControl.SelectedIndex = 0;
        }

        private void ImageChangedEvent(object sender, NodeCollectionChangedEventArgs e) {
            if (_isDispatcherInactive == false) return;
            ImageProperty property = sender as ImageProperty;

            this.Dispatcher.BeginInvoke(new Action(() => {
                _isDispatcherInactive = false;
                _mutex.WaitOne();
                ImageProperty propertyClone = property.Clone() as ImageProperty;
                propertyClone.UID = property.UID;
                Operation parentOperation = propertyClone.ParentOperation as Operation;
                foreach (OperationDisplayTabItem item in this.DisplayTabControl.Items) {
                    if (item.Operation == parentOperation) {
                        foreach (DisplayTabItem display in (item.Content as TabControl).Items) {
                            if (display.ImageProperty.UID == propertyClone.UID) {
                                display.UpdateImage(propertyClone);
                            }
                        }
                    }
                }
                _mutex.ReleaseMutex();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void Hooks_DispatcherInactive(object sender, EventArgs e) {
            _isDispatcherInactive = true;
        }
    }
}
