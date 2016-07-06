using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Core.Manager;
using System.Diagnostics;
using System.IO;

namespace ns.Core.Test {

    [TestClass]
    public class CoreSystemTest {

        [TestMethod]
        public void CoreSystem_AddAndSaveProject() {
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();

            using (MemoryStream projectStream = new MemoryStream()) {
                projectManager.Save(projectStream);
                projectStream.Position = 0;
                StreamReader reader = new StreamReader(projectStream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public void CoreSystem_InitializeFinalize() {
            CoreSystem.Close();
        }
    }
}