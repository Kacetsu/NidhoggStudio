namespace ns.Base.Imaging {

    public class Histogram {

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
        public int[] BlueValues { get; private set; }

        /// <summary>
        /// Gets the gray values.
        /// </summary>
        /// <value>
        /// The gray values.
        /// </value>
        public int[] GrayValues { get; private set; }

        /// <summary>
        /// Gets the green values.
        /// </summary>
        /// <value>
        /// The green values.
        /// </value>
        public int[] GreenValues { get; private set; }

        /// <summary>
        /// Gets the red values.
        /// </summary>
        /// <value>
        /// The red values.
        /// </value>
        public int[] RedValues { get; private set; }

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
        /// Generates the histogram.
        /// </summary>
        /// <param name="imageData">The image data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bytesPerPixel">The bytes per pixel.</param>
        protected virtual void GenerateHistogram(byte[] imageData, int width, int height, int stride, byte bytesPerPixel) {
            RedValues = null;
            GreenValues = null;
            BlueValues = null;
            GrayValues = null;

            byte bpp = bytesPerPixel;

            if (bytesPerPixel >= 3) {
                RedValues = new int[256];
                GreenValues = new int[256];
                BlueValues = new int[256];
            } else if (bytesPerPixel == 1) {
                GrayValues = new int[256];
            }

            unsafe
            {
                fixed (byte* ptr = imageData) {
                    if (bpp == 1) {
                        for (int y = 0, c = 0; y < height; y++) {
                            for (int x = 0; x < width; x++, c++) {
                                GrayValues[ptr[c]]++;
                            }
                            c += stride - width;
                        }
                    } else if (bpp >= 3) {
                        for (int y = 0, p = 0; y < height; y++) {
                            for (int x = 0; x < width; x++, p += bpp) {
                                byte r = ptr[p + 2];
                                byte g = ptr[p + 1];
                                byte b = ptr[p];
                                RedValues[r]++;
                                GreenValues[g]++;
                                BlueValues[b]++;
                            }
                            p += stride - width * bpp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Smoothes the histogram.
        /// </summary>
        /// <param name="originalValues">The original values.</param>
        /// <returns></returns>
        protected int[] SmoothHistogram(int[] originalValues) {
            int[] smoothedValues = new int[originalValues.Length];

            double[] mask = new double[] { 0.25, 0.5, 0.25 };

            for (int bin = 1; bin < originalValues.Length - 1; bin++) {
                double smoothedValue = 0;
                for (int i = 0; i < mask.Length; i++) {
                    smoothedValue += originalValues[bin - 1 + i] * mask[i];
                }
                smoothedValues[bin] = (int)smoothedValue;
            }

            return smoothedValues;
        }
    }
}