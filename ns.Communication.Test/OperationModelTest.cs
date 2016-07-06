using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Plugins;
using ns.Communication.Models;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Communication.Test {

    [TestClass]
    public class OperationModelTest {

        [TestMethod]
        public void OperationModel_SerializeDeserialize() {
            Operation operation = new Operation("TestOperation");
            DummyTool tool = new DummyTool();
            operation.AddChild(tool);

            OperationModel operationModel = new OperationModel(operation);
            using (MemoryStream stream = new MemoryStream()) {
                DataContractSerializer serializer = new DataContractSerializer(typeof(OperationModel));
                serializer.WriteObject(stream, operationModel);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
                stream.Position = 0;

                serializer = new DataContractSerializer(typeof(OperationModel));
                operationModel = serializer.ReadObject(stream) as OperationModel;
                Assert.IsNotNull(operationModel);
                Assert.AreEqual(operationModel.ChildTools[0].DisplayName, nameof(DummyTool));
            }
        }
    }
}