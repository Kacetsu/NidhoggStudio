using ns.Base;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ns.GUI.WPF {
    /// <summary>
    /// Interaktionslogik für EditorDisplay.xaml
    /// </summary>
    public partial class EditorDisplay : UserControl, INotifyPropertyChanged {
        private GuiManager _guiManager = null;
        private ImageProperty _lastImageProperty = null;
        private BitmapSource _bitmap = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapSource Bitmap {
            get { return _bitmap; }
            set {
                if (_bitmap != value) {
                    _bitmap = value;
                    OnPropertyChanged("Bitmap");
                }
            }
        }

        public EditorDisplay() {
            InitializeComponent();
            ImageDisplay.DataContext = this;
            Loaded += EditorDisplay_Loaded;
        }

        private void EditorDisplay_Loaded(object sender, RoutedEventArgs e) {
            if(_guiManager == null) {
                _guiManager = CoreSystem.Managers.Find(m => m.Name.Contains("GuiManager")) as GuiManager;
                _guiManager.SelectedItemChanged += _guiManager_SelectedItemChanged;
            }
        }

        private void _guiManager_SelectedItemChanged(object sender, Base.Event.NodeSelectionChangedEventArgs e) {
            if (e.SelectedNode == null) return;

            if(_lastImageProperty != null) {
                _lastImageProperty.PropertyChanged -= _lastImageProperty_PropertyChanged;
            }

            if(e.SelectedNode is Plugin) {
                bool containsNewImage = false;
                Plugin plugin = e.SelectedNode as Plugin;
                foreach(Node child in plugin.Childs) {
                    if(child is ImageProperty) {
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

            if(_lastImageProperty != null) {
                _lastImageProperty.PropertyChanged += _lastImageProperty_PropertyChanged;
            }
        }

        private void _lastImageProperty_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Value")) {
                if (_lastImageProperty.Value == null) return;

                ImageContainer container = (ImageContainer)_lastImageProperty.Value;
                BitmapSource tmpBitmap = ImageContainerToBitmapSource(container.Data, container.Width, container.Height, container.Stride, container.BytesPerPixel);
                tmpBitmap.Freeze();
                Bitmap = tmpBitmap;
            }
        }

        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

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
