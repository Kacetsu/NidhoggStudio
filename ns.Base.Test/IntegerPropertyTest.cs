using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class IntegerPropertyTest {
        private IntegerProperty _property;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _property = null;
        }

        /// <summary>
        /// Integers the property_ in tolerance negative_ false.
        /// </summary>
        [TestMethod]
        public void IntegerProperty_InToleranceNegative_False() {
            _property = new IntegerProperty(-20, 0, 30);
            Assert.IsFalse(_property.InTolerance);
        }

        /// <summary>
        /// Integers the property_ in tolerance negative_ true.
        /// </summary>
        [TestMethod]
        public void IntegerProperty_InToleranceNegative_True() {
            _property = new IntegerProperty(-20, -30, 30);
            Assert.IsTrue(_property.InTolerance);
        }

        /// <summary>
        /// Integers the property_ in tolerance positive_ false.
        /// </summary>
        [TestMethod]
        public void IntegerProperty_InTolerancePositive_False() {
            _property = new IntegerProperty(40, 0, 30);
            Assert.IsFalse(_property.InTolerance);
        }

        /// <summary>
        /// Integers the property_ in tolerance positive_ true.
        /// </summary>
        [TestMethod]
        public void IntegerProperty_InTolerancePositive_True() {
            _property = new IntegerProperty(20, 0, 30);
            Assert.IsTrue(_property.InTolerance);
        }

        /// <summary>
        /// Integers the property_ serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void IntegerProperty_Serialize_Deserialize() {
            _property = new IntegerProperty(20, 0, 30);
            IntegerProperty newProperty = null;

            DataContractSerializer serializer = new DataContractSerializer(_property.GetType());
            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _property);
                stream.Position = 0;

                newProperty = serializer.ReadObject(stream) as IntegerProperty;

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