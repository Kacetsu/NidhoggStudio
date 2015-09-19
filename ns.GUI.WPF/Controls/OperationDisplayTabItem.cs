using ns.Base.Log;
using ns.Base.Plugins;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationDisplayTabItem"/> class.
        /// </summary>
        public OperationDisplayTabItem() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationDisplayTabItem"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public OperationDisplayTabItem(Operation operation) {
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.Header = operation.Name;
            this.Unloaded += OperationDisplayTabItem_Unloaded;
            _operation = operation;
            operation.PropertyChanged += HandlePropertyChanged;
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
        /// Closes this instance.
        /// </summary>
        public void Close() {
            if (_operation != null) {
                _operation.PropertyChanged -= HandlePropertyChanged;
                _operation = null;
            }
            foreach (DisplayTabItem item in ((TabControl)this.Content).Items)
                item.Close();
            ((TabControl)this.Content).Items.Clear();
        }

        /// <summary>
        /// Handles the Unloaded event of the OperationDisplayTabItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OperationDisplayTabItem_Unloaded(object sender, RoutedEventArgs e) {
            Close();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name") {
                this.Header = _operation.Name;
            }
        }
    }
}
