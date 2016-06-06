using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Core.Manager;
using System;
using System.Diagnostics;
using System.IO;

namespace ns.Core.Test {

    [TestClass]
    public class CoreSystemTest {

        [TestMethod]
        public void CoreSystem_Initialize() {
            Assert.IsTrue(CoreSystem.Initialize());
        }

        [TestMethod]
        public void CoreSystem_AddAndSaveProject() {
            Assert.IsTrue(CoreSystem.Initialize());

            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains(nameof(ProjectManager))) as ProjectManager;

            using (MemoryStream projectStream = new MemoryStream()) {
                projectManager.Save(projectStream);
                projectStream.Position = 0;
                StreamReader reader = new StreamReader(projectStream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }
    }
}