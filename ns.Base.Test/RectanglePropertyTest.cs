using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class RectanglePropertyTest {
        private RectangleProperty _property;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _property = null;
        }

        /// <summary>
        /// Rectangles the property_ serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void RectangleProperty_Serialize_Deserialize() {
            _property = new RectangleProperty(new Rectangle(1.0d, 1.1d, 2.0d, 2.1d));
            RectangleProperty newProperty = null;

            DataContractSerializer serializer = new DataContractSerializer(_property.GetType());
            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _property);
                stream.Position = 0;

                newProperty = serializer.ReadObject(stream) as RectangleProperty;

#if DEBUG
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream)) {
                    Trace.WriteLine(reader.ReadToEnd());
                }
#endif // DEBUG
            }

            Assert.AreEqual(newProperty.Id, _property.Id);
            Assert.AreEqual(newProperty.X, _property.X);
            Assert.AreEqual(newProperty.Y, _property.Y);
            Assert.AreEqual(newProperty.Width, _property.Width);
            Assert.AreEqual(newProperty.Height, _property.Height);
        }
    }
}