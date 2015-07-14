using ns.Base;
using ns.Base.Event;
using ns.Base.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ns.GUI.WPF {
    public class GuiManager : BaseManager {

        public delegate void SelectionChangedHandler(object sender, NodeSelectionChangedEventArgs e);

        public virtual event SelectionChangedHandler SelectedItemChanged;

        private Node _selectedNode;

        /// <summary>
        /// Gets the selected node.
        /// </summary>
        /// <value>
        /// The selected node.
        /// </value>
        public Node SelectedNode {
            get { return _selectedNode; }
        }

        /// <summary>
        /// Selects the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void SelectNode(Node node) {
            _selectedNode = node;
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new NodeSelectionChangedEventArgs(node));
        }

        /// <summary>
        /// Sets the language dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public static void SetLanguageDictionary(ResourceDictionary dictionary) {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString()) {
                case "de-DE":
                    dict.Source = new Uri("/ns.GUI.WPF;component/Languages/de_DE.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("/ns.GUI.WPF;component/Languages/en_US.xaml", UriKind.Relative);
                    break;
            }
            dictionary.MergedDictionaries.Add(dict);
        }
    }
}
