using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.GUI.WPF.Controls;
using System;
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
    /// Interaktionslogik für EditorDisplay.xaml
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
        /// Tritt ein, wenn sich ein Eigenschaftswert ändert.
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

        private void AddOverlayProperties(Tool parent) {
            if (parent == null) return;
            if (_rectangles == null) _rectangles = new List<OverlayRectangle>();

            foreach (Property child in parent.Items.Where(c => c is Property)) {
                if (!child.IsOutput && child is RectangleProperty) {
                    OverlayRectangle overlay = new OverlayRectangle(child as RectangleProperty, ImageCanvas);
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
                BitmapSource bitmapSource = ImageContainerToBitmapSource(imageContainer.Data, imageContainer.Width, imageContainer.Height, imageContainer.Stride, imageContainer.BytesPerPixel);
                bitmapSource.Freeze();
                Image = bitmapSource;
                ImageHeight = Image.Height;
                ImageWidth = Image.Width;
            }
        }

        private BitmapSource ImageContainerToBitmapSource(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            PixelFormat pixelFormat = PixelFormats.Bgr24;

            if (bytesPerPixel == 1)
                pixelFormat = PixelFormats.Gray8;
            else if (bytesPerPixel == 4)
                pixelFormat = PixelFormats.Bgr32;

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