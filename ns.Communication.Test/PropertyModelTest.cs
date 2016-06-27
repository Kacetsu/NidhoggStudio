using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins;
using ns.Base.Plugins.Properties;
using ns.Communication.CommunicationModels;
using ns.Communication.CommunicationModels.Properties;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Communication.Test {

    [TestClass]
    public class PropertyModelTest {

        [TestMethod]
        public void PropertyModel_SerializeDeserialize() {
            DoubleProperty doubleProperty = new DoubleProperty("DoubleProperty", 20.1, 20.0, 21.2);
            PropertyModel propertyModel = new PropertyModel(doubleProperty);

            using (MemoryStream stream = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(PropertyModel));
                serializer.WriteObject(stream, propertyModel);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
                stream.Position = 0;

                serializer = new DataContractSerializer(typeof(PropertyModel));
                propertyModel = serializer.ReadObject(stream) as PropertyModel;
                Assert.IsNotNull(propertyModel);
                Assert.AreEqual(20.1, (propertyModel.Property as DoubleProperty).Value);
            }
        }
    }
}