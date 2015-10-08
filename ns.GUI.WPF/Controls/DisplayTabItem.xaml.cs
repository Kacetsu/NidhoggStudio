using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ns.GUI.WPF.Controls {
    /// <summary>
    /// Interaction logic for DisplayTabItem.xaml
    /// </summary>
    public partial class DisplayTabItem : TabItem, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private ImageProperty _imageProperty;
        private List<OverlayRectangle> _rectangles;
        private TabControl _parentControl;
        
        private double _scalingFactor = 1.0;
        private string _scalingFactorString = string.Empty;
        private bool _isHistogramEnabled = false;

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
        /// Gets or sets the scaling factor.
        /// </summary>
        public double ScalingFactor {
            get { return _scalingFactor; }
            set {
                _scalingFactor = value;
                ScalingFactorString = (_scalingFactor * 100).ToString() + "%";
                OnPropertyChanged("ScalingFactor");
            }
        }
        
        /// <summary>
        /// Gets or sets the scaling factor as string.
        /// @Warning: Should only be used as binding.
        /// </summary>
        public string ScalingFactorString {
            get { return _scalingFactorString; }
            set {
                _scalingFactorString = value;
                OnPropertyChanged("ScalingFactorString");
            }
        }

        public bool IsHistogramEnabled {
            get { return _isHistogramEnabled; }
            set {
                _isHistogramEnabled = value;

                string iconUrl = "/ns.GUI.WPF;component/Images/Histogram_Enabled.png";
                if (!_isHistogramEnabled)
                    iconUrl = "/ns.GUI.WPF;component/Images/Histogram.png";

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(iconUrl, UriKind.Relative);
                image.EndInit();

                HistogramToggleButtonImage.Source = image;

                OnPropertyChanged("IsHistogramEnabled");
                OnPropertyChanged("HistogramVisibility");
            }
        }

        public Visibility HistogramVisibility {
            get {
                return IsHistogramEnabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the histogram.
        /// </summary>
        /// <value>
        /// The histogram.
        /// </value>
        public Histogram Histogram {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update histogram enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is update histogram enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsUpdateHistogramEnabled {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayTabItem"/> class.
        /// </summary>
        /// <param name="imageProperty">The image property.</param>
        public DisplayTabItem(ImageProperty imageProperty)
            : base() {
            ScalingFactor = 1.0;
            InitializeComponent();
            _imageProperty = imageProperty;
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.Header = _imageProperty.ParentTool.Name + " - " + _imageProperty.Name;
            _imageProperty.ParentTool.PropertyChanged += HandleParentPropertyChanged;
            this.DataContext = this;
            this.HistogramGray.DataContext = Histogram;
            this.HistogramAllGray.DataContext = Histogram;
            this.HistogramRed.DataContext = Histogram;
            this.HistogramAllRed.DataContext = Histogram;
            this.HistogramGreen.DataContext = Histogram;
            this.HistogramAllGreen.DataContext = Histogram;
            this.HistogramBlue.DataContext = Histogram;
            this.HistogramAllBlue.DataContext = Histogram;
            this.IsUpdateHistogramEnabled = false;
            SetOverlayProperties(imageProperty);
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close() {
            if (_imageProperty != null) {
                _imageProperty.ParentTool.PropertyChanged -= HandleParentPropertyChanged;
                _imageProperty = null;
            }
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="control">The control.</param>
        public void SetParent(TabControl control) {
            _parentControl = control;
            _parentControl.SelectionChanged += ParentSelectionChanged;
        }

        private void ParentSelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.IsUpdateHistogramEnabled = (_parentControl.SelectedItem == this);
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="image">The image.</param>
        public void UpdateImage(ImageProperty image) {
            if (image.Value != null) {
                ImageContainer container = (ImageContainer)image.Value;
                this.ImageDisplay.Source = LoadImage(container.Data, container.Width, container.Height, container.Stride, container.BytesPerPixel);

                this.ImageDisplay.Width = container.Width;
                this.ImageDisplay.Height = container.Height;
                this.ImageCanvas.Width = container.Width;
                this.ImageCanvas.Height = container.Height;
            }
        }

        private void HandleParentPropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.Dispatcher.BeginInvoke(new Action(() => {
                if (e.PropertyName == "Name") {
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

            if (this.IsHistogramEnabled && this.IsUpdateHistogramEnabled) {
                if (Histogram == null) {
                    Histogram = new Histogram(imageData, width, height, stride, bytesPerPixel);
                    this.HistogramGray.DataContext = Histogram;
                    this.HistogramAllGray.DataContext = Histogram;
                    this.HistogramRed.DataContext = Histogram;
                    this.HistogramAllRed.DataContext = Histogram;
                    this.HistogramGreen.DataContext = Histogram;
                    this.HistogramAllGreen.DataContext = Histogram;
                    this.HistogramBlue.DataContext = Histogram;
                    this.HistogramAllBlue.DataContext = Histogram;
                } else
                    Histogram.Update(imageData, width, height, stride, bytesPerPixel);

                if (bytesPerPixel == 1) {
                    this.HistrogramAllGrid.Visibility = System.Windows.Visibility.Collapsed;
                    this.HistogramRedGrid.Visibility = System.Windows.Visibility.Collapsed;
                    this.HistogramBlueGrid.Visibility = System.Windows.Visibility.Collapsed;
                    this.HistogramGreenGrid.Visibility = System.Windows.Visibility.Collapsed;
                    this.HistogramGrayLuminanceGrid.Visibility = System.Windows.Visibility.Visible;
                    this.HistoRow1.Height = new GridLength(0);
                    this.HistoRow2.Height = new GridLength(0);
                    this.HistoRow3.Height = new GridLength(0);
                    this.HistoRow4.Height = new GridLength(0);
                    this.HistoRow5.Height = new GridLength(100, GridUnitType.Star);
                } else {
                    this.HistrogramAllGrid.Visibility = System.Windows.Visibility.Visible;
                    this.HistogramRedGrid.Visibility = System.Windows.Visibility.Visible;
                    this.HistogramBlueGrid.Visibility = System.Windows.Visibility.Visible;
                    this.HistogramGreenGrid.Visibility = System.Windows.Visibility.Visible;
                    this.HistogramGrayLuminanceGrid.Visibility = System.Windows.Visibility.Collapsed;
                    this.HistoRow1.Height = new GridLength(10, GridUnitType.Star);
                    this.HistoRow2.Height = new GridLength(20, GridUnitType.Star);
                    this.HistoRow3.Height = new GridLength(20, GridUnitType.Star);
                    this.HistoRow4.Height = new GridLength(20, GridUnitType.Star);
                    this.HistoRow5.Height = new GridLength(0);
                }
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

        private void SetOverlayProperties(ImageProperty imageProperty) {
            if(imageProperty.Parent is Tool) {
                Tool parent = imageProperty.Parent as Tool;
                foreach(ns.Base.Plugins.Properties.Property child in parent.Childs.Where(c => c is ns.Base.Plugins.Properties.Property)) {
                    if(child is RectangleProperty && !child.IsOutput) {
                        if (_rectangles == null) _rectangles = new List<OverlayRectangle>();
                        OverlayRectangle overlay = new OverlayRectangle(child as RectangleProperty, this.ImageCanvas);
                        _rectangles.Add(overlay);
                        this.ImageCanvas.Children.Add(overlay.Rectangle);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            double newScalingFactor = _scalingFactor;
            if(sender == this.ZoomInButton) {
                if (newScalingFactor < 2.0) {
                    newScalingFactor += 0.1;
                } else {
                    newScalingFactor += 1.0;
                }
            } else if(sender == this.ZoomOutButton) {
                if(_scalingFactor <= 2.1) {
                    newScalingFactor -= 0.1;
                } else {
                    newScalingFactor -= 1.0;
                }

                if(newScalingFactor < 0.1) {
                    newScalingFactor = 0.1;
                }
            } else if(sender == this.HistogramToggleButton) {
                IsHistogramEnabled = !IsHistogramEnabled;
            }
            ScalingFactor = newScalingFactor;
        }
    }
}
