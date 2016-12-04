using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class ImagePropertyTest {
        private ImageProperty _property;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _property = null;
        }

        /// <summary>
        /// Images the property_ serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void ImageProperty_Serialize_Deserialize() {
            ImageContainer container = new ImageContainer();
            container.Data = new byte[4] { 0, 1, 2, 3 };
            container.Width = 2;
            container.Height = 2;
            container.BytesPerPixel = 1;
            container.Stride = 2;
            _property = new ImageProperty(container);
            ImageProperty newProperty = null;

            DataContractSerializer serializer = new DataContractSerializer(_property.GetType());
            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _property);
                stream.Position = 0;

                newProperty = serializer.ReadObject(stream) as ImageProperty;

#if DEBUG
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream)) {
                    Trace.WriteLine(reader.ReadToEnd());
                }
#endif // DEBUG
            }

            Assert.AreEqual(newProperty.Id, _property.Id);
            Assert.AreEqual(newProperty.Value.BytesPerPixel, _property.Value.BytesPerPixel);
            Assert.AreEqual(newProperty.Value.Stride, _property.Value.Stride);
            Assert.AreEqual(newProperty.Value.Width, _property.Value.Width);
            Assert.AreEqual(newProperty.Value.Height, _property.Value.Height);
            Assert.AreEqual(newProperty.Value.Data[0], _property.Value.Data[0]);
            Assert.AreEqual(newProperty.Value.Data[1], _property.Value.Data[1]);
            Assert.AreEqual(newProperty.Value.Data[2], _property.Value.Data[2]);
            Assert.AreEqual(newProperty.Value.Data[3], _property.Value.Data[3]);
        }
    }
}