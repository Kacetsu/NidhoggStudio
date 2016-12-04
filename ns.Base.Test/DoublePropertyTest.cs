using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class DoublePropertyTest {
        private DoubleProperty _property;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _property = null;
        }

        /// <summary>
        /// Doubles the property_ in tolerance negative_ false.
        /// </summary>
        [TestMethod]
        public void DoubleProperty_InToleranceNegative_False() {
            _property = new DoubleProperty(-20d, 0d, 30d);
            Assert.IsFalse(_property.InTolerance);
        }

        /// <summary>
        /// Doubles the property_ in tolerance negative_ true.
        /// </summary>
        [TestMethod]
        public void DoubleProperty_InToleranceNegative_True() {
            _property = new DoubleProperty(-20d, -30d, 30d);
            Assert.IsTrue(_property.InTolerance);
        }

        /// <summary>
        /// Doubles the property_ in tolerance positive_ false.
        /// </summary>
        [TestMethod]
        public void DoubleProperty_InTolerancePositive_False() {
            _property = new DoubleProperty(40d, 0d, 30d);
            Assert.IsFalse(_property.InTolerance);
        }

        /// <summary>
        /// Doubles the property_ in tolerance positive_ true.
        /// </summary>
        [TestMethod]
        public void DoubleProperty_InTolerancePositive_True() {
            _property = new DoubleProperty(20d, 0d, 30d);
            Assert.IsTrue(_property.InTolerance);
        }

        /// <summary>
        /// Doubles the property_ serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void DoubleProperty_Serialize_Deserialize() {
            _property = new DoubleProperty(20d, 0d, 30d);
            DoubleProperty newProperty = null;

            DataContractSerializer serializer = new DataContractSerializer(_property.GetType());
            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _property);
                stream.Position = 0;

                newProperty = serializer.ReadObject(stream) as DoubleProperty;

#if DEBUG
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream)) {
                    Trace.WriteLine(reader.ReadToEnd());
                }
#endif // DEBUG
            }

            Assert.AreEqual(newProperty.Id, _property.Id);
            Assert.AreEqual(newProperty.Value, _property.Value);
            Assert.AreEqual(newProperty.Min, _property.Min);
            Assert.AreEqual(newProperty.Max, _property.Max);
        }
    }
}