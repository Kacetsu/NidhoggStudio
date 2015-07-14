using ns.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : UserControl {
        public Shell() {
            InitializeComponent();
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) {
                List<string> autoCompleteCommands = CoreSystem.Shell.GetAutocompleteCommands(inputBox.Text);

                if (autoCompleteCommands.Count > 0) {
                    inputBox.IsDropDownOpen = true;
                    inputBox.ItemsSource = autoCompleteCommands;
                }
            } else {
                CoreSystem.Shell.Execute(inputBox.Text);
            }
        }
    }
}
