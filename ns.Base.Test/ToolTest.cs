using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Base.Test.Dummies;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace ns.Base.Test {

    [TestClass]
    public class ToolTest {
        private DummyTool _tool;

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        [TestCleanup]
        public void Cleanup() {
            _tool = null;
        }

        /// <summary>
        /// Tool_s the initialize_ true.
        /// </summary>
        [TestMethod]
        public void Tool_Initialize_True() {
            _tool = new DummyTool();

            Assert.IsTrue(_tool.TryInitialize());
        }

        /// <summary>
        /// Tool_s the serialize_ deserialize.
        /// </summary>
        [TestMethod]
        public void Tool_Serialize_Deserialize() {
            _tool = new DummyTool();
            _tool.Initialize();
            DummyTool newTool = null;

            DataContractSerializer serializer = new DataContractSerializer(_tool.GetType());

            using (MemoryStream stream = new MemoryStream()) {
                serializer.WriteObject(stream, _tool);
                stream.Position = 0;

                newTool = serializer.ReadObject(stream) as DummyTool;

#if DEBUG
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream)) {
                    Trace.WriteLine(reader.ReadToEnd());
                }
#endif // DEBUG
            }

            Assert.AreEqual(newTool.Id, _tool.Id);
            Assert.AreEqual(newTool.Items.Count, 1);
        }
    }
}