using ns.Base.Plugins.Properties;
using ns.Communication.Models;
using ns.Communication.Models.Properties;
using ns.GUI.WPF.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF {

    /// <summary>
    /// Logic for <see cref="EditorDisplay"/>.
    /// </summary>
    public partial class EditorDisplay : UserControl, INotifyPropertyChanged {
        private BitmapSource _image = null;
        private double _imageHeight = 0;
        private double _imageWidth = 0;

        private List<OverlayRectangle> _rectangles;
        private double _scalingFactor = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorDisplay"/> class.
        /// </summary>
        public EditorDisplay() {
            InitializeComponent();
            ImageDisplay.DataContext = this;
            ImageCanvas.DataContext = this;
            Loaded += EditorDisplay_Loaded;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public BitmapSource Image {
            get { return _image; }
            set {
                if (_image != value) {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public double ImageHeight {
            get { return _imageHeight; }
            set {
                if (_imageHeight != value) {
                    _imageHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public double ImageWidth {
            get { return _imageWidth; }
            set {
                if (_imageWidth != value) {
                    _imageWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the scaling factor.
        /// </summary>
        /// <value>
        /// The scaling factor.
        /// </value>
        public double ScalingFactor {
            get { return _scalingFactor; }
            set {
                if (_scalingFactor != value) {
                    _scalingFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        private void AddOverlayProperties(ToolModel parent) {
            if (parent == null) return;
            if (_rectangles == null) _rectangles = new List<OverlayRectangle>();

            foreach (PropertyModel child in parent.Properties.Where(c => c.Property is Property)) {
                if (!child.Property.IsOutput && child.Property is RectangleProperty) {
                    OverlayRectangle overlay = new OverlayRectangle(child.Property as RectangleProperty, ImageCanvas);
                    _rectangles.Add(overlay);
                    ImageCanvas.Children.Add(overlay.Rectangle);
                }
            }
        }

        private void EditorDisplay_Loaded(object sender, RoutedEventArgs e) {
            FrontendManager.Instance.PropertyChanged += FrontendManager_PropertyChanged;
        }

        private void FrontendManager_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(FrontendManager.SelectedPluginImage))) {
                ImageContainer imageContainer = FrontendManager.SelectedPluginImage.Value;
                BitmapSource bitmapSource = null;
                if (imageContainer.Data?.Length > 0) {
                    bitmapSource = ImageContainerToBitmapSource(imageContainer.Data, imageContainer.Width, imageContainer.Height, imageContainer.Stride, imageContainer.BytesPerPixel);
                } else {
                    bitmapSource = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Gray8, null, new byte[1], 1);
                }
                bitmapSource.Freeze();
                Image = bitmapSource;
                ImageHeight = Image.Height;
                ImageWidth = Image.Width;
            } else if (e.PropertyName.Equals(nameof(FrontendManager.SelectedModel))) {
                ImageCanvas.Children.Clear();
                ImageCanvas.Children.Add(ImageDisplay);
                AddOverlayProperties(FrontendManager.SelectedModel as ToolModel);
            }
        }

        private BitmapSource ImageContainerToBitmapSource(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            PixelFormat pixelFormat = PixelFormats.Bgr24;

            switch (bytesPerPixel) {
                case 1:
                pixelFormat = PixelFormats.Gray8;
                break;

                case 3:
                pixelFormat = PixelFormats.Bgr24;
                break;

                case 4:
                pixelFormat = PixelFormats.Bgr32;
                break;
            }

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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}