using ns.Base;
using ns.Base.Plugins;
using ns.Core;
using ns.Core.Manager;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ns.GUI.WPF.Windows {

    /// <summary>
    /// Interaktionslogik für AddNewElementDialog.xaml
    /// </summary>
    public partial class AddNewElementDialog : BaseWindow {
        private GuiManager _guiManager;
        private ProjectManager _projectManager;
        private List<KeyValuePair<string, ListBox>> _listBoxes;
        private Node _lastAddedNode = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewElementDialog"/> class.
        /// </summary>
        public AddNewElementDialog() {
            InitializeComponent();
            _listBoxes = new List<KeyValuePair<string, ListBox>>();
            this.Loaded += HandleLoaded;
            this.Closing += HandleClosing;
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (_lastAddedNode != null)
                _guiManager.SelectNode(_lastAddedNode);
        }

        /// <summary>
        /// Adds the tool.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Selected tool is NULL!
        /// or
        /// Selected operation is NULL!
        /// </exception>
        private void AddTool() {
            ListBox view = null;
            if (this.AnyTabs.SelectedItem == this.MainToolsTab) {
                TabItem selectedTab = this.ToolTabs.SelectedItem as TabItem;
                view = selectedTab.Content as ListBox;

                if (view != null) {
                    Tool tool = view.SelectedItem as Tool;

                    if (tool == null)
                        return;

                    if (_guiManager.SelectedNode != null && _guiManager.SelectedNode is Tool) {
                        _guiManager.SelectNode(_guiManager.SelectedNode.Parent);
                    }

                    if (_guiManager.SelectedNode == null) {
                        ProjectManager manager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;
                        if (manager.Configuration.Operations.Count > 0)
                            _guiManager.SelectNode(manager.Configuration.Operations[0]);
                    }

                    if (_guiManager.SelectedNode != null && _guiManager.SelectedNode is Operation) {
                        _lastAddedNode = new Tool(tool);
                        _projectManager.Add(_lastAddedNode as Tool, _guiManager.SelectedNode as Operation);
                    }
                }
            } else if (this.AnyTabs.SelectedItem == this.MainOperationsTabs) {
                view = this.ListViewAllOperations;

                Operation operation = view.SelectedItem as Operation;

                if (operation == null)
                    return;

                _lastAddedNode = new Operation(operation);
                _projectManager.Add(_lastAddedNode as Operation);
                _guiManager.SelectNode(_lastAddedNode);
            }
        }

        /// <summary>
        /// Handles the loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HandleLoaded(object sender, RoutedEventArgs e) {
            PluginManager pluginManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(PluginManager))) as PluginManager;
            _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(GuiManager))) as GuiManager;
            _projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;

            if (pluginManager == null) {
                Base.Log.Trace.WriteLine("Could not find ToolManager instance in CoreSystem!", TraceEventType.Error);
                return;
            }

            if (_guiManager == null) {
                Base.Log.Trace.WriteLine("Could not find NodeSelectionManager instance in CoreSystem!", TraceEventType.Error);
                return;
            }

            foreach (Plugin plugin in pluginManager.Nodes) {
                Tool tool = plugin as Tool;
                Operation operation = plugin as Operation;

                if (tool != null) {
                    ListViewAll.Items.Add(plugin);

                    bool contains = false;

                    foreach (KeyValuePair<string, ListBox> pair in _listBoxes) {
                        if (pair.Key == tool.Category) {
                            pair.Value.Items.Add(tool);
                            contains = true;
                            break;
                        }
                    }

                    if (!contains) {
                        TabItem item = null;
                        string tabName = "Tool" + tool.Category.Replace(' ', '_');
                        foreach (TabItem it in ToolTabs.Items) {
                            if (it.Name == tabName) {
                                item = it;
                                break;
                            }
                        }

                        if (item == null) {
                            item = new TabItem();
                            item.Name = tabName;
                            item.Header = tool.Category;
                            ToolTabs.Items.Add(item);
                        }
                        ListBox box = new ListBox();
                        item.Content = box;
                        _listBoxes.Add(new KeyValuePair<string, ListBox>(tool.Category, box));
                        box.Items.Add(tool);
                    }
                } else if (operation != null) {
                    ListViewAllOperations.Items.Add(operation);
                }
            }
        }

        private void HandleButtonClick(object sender, RoutedEventArgs e) {
            if (sender == AddButton)
                AddTool();
        }
    }
}