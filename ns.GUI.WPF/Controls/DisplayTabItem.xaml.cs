using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaction logic for DisplayTabItem.xaml
    /// </summary>
    public partial class DisplayTabItem : TabItem, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private ImageProperty _imageProperty;
        private bool _isFitToScreen = true;

        /// <summary>
        /// Gets the operation.
        /// </summary>
        /// <value>
        /// The operation.
        /// </value>
        public ImageProperty ImageProperty {
            get { return _imageProperty; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fit to screen.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fit to screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsFitToScreen {
            get { return _isFitToScreen; }
            set { 
                _isFitToScreen = value;
                this.ImageDisplay.StretchDirection = StretchDirection.Both;
                if (_isFitToScreen) {
                    this.ImageDisplay.Stretch = Stretch.Uniform;
                    this.ImageScrollArea.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    this.ImageScrollArea.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                } else {
                    this.ImageDisplay.Stretch = Stretch.None;
                    this.ImageScrollArea.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                    this.ImageScrollArea.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                OnPropertyChanged("IsFitToScreen");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTabItem"/> class.
        /// </summary>
        /// <param name="imageProperty">The image property.</param>
        public DisplayTabItem(ImageProperty imageProperty)
            : base() {
                InitializeComponent();
                _imageProperty = imageProperty;
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.Header = _imageProperty.ParentTool.Name + " - " + _imageProperty.Name;
            _imageProperty.ParentTool.NodeChanged += ImageParentPropertyChanged;
            this.DataContext = this;
            this.IsFitToScreen = true;
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        public void UpdateImage(ImageProperty image) {
            if (image.Value != null) {
                ImageContainer container = (ImageContainer)image.Value;
                this.ImageDisplay.Source = LoadImage(container.Data, container.Width, container.Height, container.Stride, container.BytesPerPixel);
            }
        }

        /// <summary>
        /// Operations the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Base.Event.NodeChangedEventArgs"/> instance containing the event data.</param>
        private void ImageParentPropertyChanged(object sender, Base.Event.NodeChangedEventArgs e) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                if (e.Name == "Name") {
                    this.Header = _imageProperty.ParentTool.Name + " - " + _imageProperty.Name;
                }
            }));
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        /// <returns></returns>
        private BitmapSource LoadImage(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {

            PixelFormat pixelFormat = PixelFormats.Bgr24;

            if (bytesPerPixel == 1)
                pixelFormat = PixelFormats.Gray8;

            return BitmapSource.Create(
                width,
                height,
                96,
                96,
                pixelFormat,
                null,
                imageData,
                stride);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
