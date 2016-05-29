using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
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
    /// Interaktionslogik für EditorDisplay.xaml
    /// </summary>
    public partial class EditorDisplay : UserControl, INotifyPropertyChanged {
        private GuiManager _guiManager = null;
        private ImageProperty _lastImageProperty = null;
        private BitmapSource _bitmap = null;
        private List<OverlayRectangle> _rectangles;
        private double _imageWidth = 0;
        private double _imageHeight = 0;
        private double _scalingFactor = 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSource Bitmap {
            get { return _bitmap; }
            set {
                if (_bitmap != value) {
                    _bitmap = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ImageWidth {
            get { return _imageWidth; }
            set {
                if (_imageWidth != value) {
                    _imageWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ImageHeight {
            get { return _imageHeight; }
            set {
                if (_imageHeight != value) {
                    _imageHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ScalingFactor {
            get { return _scalingFactor; }
            set {
                if (_scalingFactor != value) {
                    _scalingFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        public EditorDisplay() {
            InitializeComponent();
            ImageDisplay.DataContext = this;
            ImageCanvas.DataContext = this;
            Loaded += EditorDisplay_Loaded;
        }

        private void AddOverlayProperties(Tool parent) {
            if (parent == null) return;
            if (_rectangles == null) _rectangles = new List<OverlayRectangle>();

            foreach (Property child in parent.Childs.Where(c => c is Property)) {
                if (!child.IsOutput && child is RectangleProperty) {
                    OverlayRectangle overlay = new OverlayRectangle(child as RectangleProperty, ImageCanvas);
                    _rectangles.Add(overlay);
                    ImageCanvas.Children.Add(overlay.Rectangle);
                }
            }
        }

        private void EditorDisplay_Loaded(object sender, RoutedEventArgs e) {
            if (_guiManager == null) {
                _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(GuiManager))) as GuiManager;
                _guiManager.SelectedItemChanged += _guiManager_SelectedItemChanged;
            }
        }

        private void _guiManager_SelectedItemChanged(object sender, Base.Event.NodeSelectionChangedEventArgs e) {
            if (e.SelectedNode == null) return;

            if (_rectangles != null) _rectangles.Clear();
            Image oldImageDisplay = ImageDisplay;
            ImageCanvas.Children.Clear();
            ImageCanvas.Children.Add(oldImageDisplay);

            if (_lastImageProperty != null) {
                _lastImageProperty.PropertyChanged -= _lastImageProperty_PropertyChanged;
            }

            if (e.SelectedNode is Plugin) {
                bool containsNewImage = false;
                Plugin plugin = e.SelectedNode as Plugin;
                foreach (Node child in plugin.Childs) {
                    if (child is ImageProperty) {
                        ImageProperty imageProperty = child as ImageProperty;
                        if (imageProperty.IsOutput || imageProperty.IsVisible) {
                            if (!imageProperty.IsOutput && imageProperty.ConnectedProperty != null) {
                                _lastImageProperty = imageProperty.ConnectedProperty as ImageProperty;
                            } else {
                                _lastImageProperty = imageProperty;
                            }
                            containsNewImage = true;
                            break;
                        }
                    }
                }

                if (!containsNewImage) {
                    _lastImageProperty = null;
                    ImageDisplay.Visibility = Visibility.Hidden;
                } else {
                    ImageDisplay.Visibility = Visibility.Visible;
                }
            }

            if (_lastImageProperty != null) {
                _lastImageProperty.PropertyChanged += _lastImageProperty_PropertyChanged;
                AddOverlayProperties(e.SelectedNode as Tool);
            }
        }

        private void _lastImageProperty_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals(nameof(ImageProperty.Value))) {
                if (_lastImageProperty.Value.Data == null || _lastImageProperty.Value.Data.Count() < 1) return;

                ImageContainer container = _lastImageProperty.Value;
                BitmapSource tmpBitmap = ImageContainerToBitmapSource(container.Data, container.Width, container.Height, container.Stride, container.BytesPerPixel);
                tmpBitmap.Freeze();
                Bitmap = tmpBitmap;
                ImageWidth = Bitmap.Width;
                ImageHeight = Bitmap.Height;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private BitmapSource ImageContainerToBitmapSource(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
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
    }
}