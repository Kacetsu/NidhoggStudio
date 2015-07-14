using ns.Base.Log;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ns.GUI.WPF.Controls {
    public class OperationDisplayTabItem : TabItem {
        private Operation _operation;

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public Operation Operation {
            get { return _operation; }
        }

        public OperationDisplayTabItem() { }

        public OperationDisplayTabItem(Operation operation) {
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.Header = operation.Name;
            _operation = operation;
            operation.NodeChanged += OperationPropertyChanged;
            TabControl tabControl = new TabControl();

            try {
                ImageBrush brush = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/ns.GUI.WPF;component/Images/Blank_Background.png")));
                brush.Stretch = Stretch.None;
                brush.TileMode = TileMode.Tile;
                brush.ViewboxUnits = BrushMappingMode.Absolute;
                brush.Viewport = new Rect(0, 0, 16, 16);

                tabControl.Background = brush;
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message, ex.StackTrace, LogCategory.Error);
            }

            this.Content = tabControl;
        }

        /// <summary>
        /// Operations the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Base.Event.NodeChangedEventArgs"/> instance containing the event data.</param>
        private void OperationPropertyChanged(object sender, Base.Event.NodeChangedEventArgs e) {
            if (e.Name == "Name") {
                this.Header = _operation.Name;
            }
        }
    }
}
