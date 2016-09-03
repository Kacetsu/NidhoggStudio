using System.Collections.Generic;
using System.Linq;

namespace ns.Base.Imaging {

    public class Histogram {
        private int[] _blueValues = null;
        private int[] _grayValues = null;
        private int[] _greenValues = null;
        private int[] _redValues = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Histogram"/> class.
        /// </summary>
        public Histogram() { }

        /// <summary>
        /// Gets the blue values.
        /// </summary>
        /// <value>
        /// The blue values.
        /// </value>
        public ICollection<int> BlueValues => _blueValues?.ToList();

        /// <summary>
        /// Gets the gray values.
        /// </summary>
        /// <value>
        /// The gray values.
        /// </value>
        public ICollection<int> GrayValues => _grayValues?.ToList();

        /// <summary>
        /// Gets the green values.
        /// </summary>
        /// <value>
        /// The green values.
        /// </value>
        public ICollection<int> GreenValues => _greenValues?.ToList();

        /// <summary>
        /// Gets the red values.
        /// </summary>
        /// <value>
        /// The red values.
        /// </value>
        public ICollection<int> RedValues => _redValues?.ToList();

        /// <summary>
        /// Updates the specified image data.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        public virtual void Update(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            GenerateHistogram(imageData, width, height, stride, bytesPerPixel);
        }

        /// <summary>
        /// Smoothes the histogram.
        /// </summary>
        /// <param name="originalValues">The original values.</param>
        /// <returns></returns>
        protected static ICollection<int> SmoothHistogram(ICollection<int> originalValues) {
            int[] sourceValues = originalValues.ToArray();
            int[] smoothedValues = new int[sourceValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < sourceValues.Length - 1; bin++) {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++) {
                    smoothedValue += sourceValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues.ToList();
        }

        /// <summary>
        /// Generates the histogram.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        protected virtual void GenerateHistogram(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            _redValues = null;
            _greenValues = null;
            _blueValues = null;
            _grayValues = null;

            byte bpp = bytesPerPixel;

            if (bytesPerPixel >= 3) {
                _redValues = new int[256];
                _greenValues = new int[256];
                _blueValues = new int[256];
            } else if (bytesPerPixel == 1) {
                _grayValues = new int[256];
            }

            unsafe
            {
                fixed (byte* ptr = imageData) {
                    if (bpp == 1) {
                        for (int y = 0, c = 0; y < height; y++) {
                            for (int x = 0; x < width; x++, c++) {
                                _grayValues[ptr[c]]++;
                            }
                            c += stride - width;
                        }
                    } else if (bpp >= 3) {
                        for (int y = 0, p = 0; y < height; y++) {
                            for (int x = 0; x < width; x++, p += bpp) {
                                byte r = ptr[p + 2];
                                byte g = ptr[p + 1];
                                byte b = ptr[p];
                                _redValues[r]++;
                                _greenValues[g]++;
                                _blueValues[b]++;
                            }
                            p += stride - width * bpp;
                        }
                    }
                }
            }
        }
    }
}