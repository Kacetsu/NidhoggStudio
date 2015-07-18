using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace ns.GUI.WPF {
    public class Histogram : ns.Base.Imaging.Histogram, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private PointCollection _redPoints;
        private PointCollection _greenPoints;
        private PointCollection _bluePoints;
        private PointCollection _grayPoints;

        /// <summary>
        /// Gets the red points.
        /// </summary>
        /// <value>
        /// The red points.
        /// </value>
        public PointCollection RedPoints {
            get { return _redPoints; }
            private set {
                _redPoints = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("RedPoints"));
                }
            }
        }

        /// <summary>
        /// Gets the green points.
        /// </summary>
        /// <value>
        /// The green points.
        /// </value>
        public PointCollection GreenPoints {
            get { return _greenPoints; }
            private set {
                _greenPoints = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("GreenPoints"));
                }
            }
        }

        /// <summary>
        /// Gets the blue points.
        /// </summary>
        /// <value>
        /// The blue points.
        /// </value>
        public PointCollection BluePoints {
            get { return _bluePoints; }
            private set {
                _bluePoints = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("BluePoints"));
                }
            }
        }

        /// <summary>
        /// Gets the gray points.
        /// </summary>
        /// <value>
        /// The gray points.
        /// </value>
        public PointCollection GrayPoints {
            get { return _grayPoints; }
            private set {
                _grayPoints = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("GrayPoints"));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Histogram"/> class.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        public Histogram(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) : base(imageData, width, height, stride, bytesPerPixel) { }

        /// <summary>
        /// Updates the specified image data.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        public override void Update(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            if (RedPoints != null) RedPoints.Clear();
            if (GreenPoints != null) GreenPoints.Clear();
            if (BluePoints != null) BluePoints.Clear();
            if (GrayPoints != null) GrayPoints.Clear();
            GenerateHistogram(imageData, width, height, stride, bytesPerPixel);
        }

        /// <summary>
        /// Generates the histogram.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        protected override void GenerateHistogram(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
 	        base.GenerateHistogram(imageData, width, height, stride, bytesPerPixel);
            switch (bytesPerPixel) {
                case 3:
                    RedPoints = GeneratePointCollection(SmoothHistogram(RedValues));
                    GreenPoints = GeneratePointCollection(SmoothHistogram(GreenValues));
                    BluePoints = GeneratePointCollection(SmoothHistogram(BlueValues));
                    break;
                default:
                    GrayPoints = GeneratePointCollection(SmoothHistogram(GrayValues));
                    break;
            }
        }

        /// <summary>
        /// Generates the point collection.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        private PointCollection GeneratePointCollection(int[] values) {
            int max = values.Max();
            PointCollection points = new PointCollection();
            // first point (lower-left corner)
            points.Add(new System.Windows.Point(0, max));
            // middle points
            for (int index = 0; index < values.Length; index++) {
                points.Add(new System.Windows.Point(index, max - values[index]));
            }
            // last point (lower-right corner)
            points.Add(new System.Windows.Point(values.Length - 1, max));
            return points;
        }
    }
}
