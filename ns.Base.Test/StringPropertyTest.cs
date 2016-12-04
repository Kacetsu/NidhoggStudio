using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class StringPropertyTest {
        private StringProperty _property;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _property = null;
        }

        /// <summary>
        /// Strings the property_ serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void StringProperty_Serialize_Deserialize() {
            _property = new StringProperty("Test");
            StringProperty newProperty = null;

            DataContractSerializer serializer = new DataContractSerializer(_property.GetType());
            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _property);
                stream.Position = 0;

                newProperty = serializer.ReadObject(stream) as StringProperty;

#if DEBUG
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream)) {
                    Trace.WriteLine(reader.ReadToEnd());
                }
#endif // DEBUG
            }

            Assert.AreEqual(newProperty.Id, _property.Id);
            Assert.AreEqual(newProperty.Value, _property.Value);
        }
    }
}