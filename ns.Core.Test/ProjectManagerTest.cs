using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Core.Manager;
using System.Diagnostics;
using System.IO;

namespace ns.Core.Test {

    [TestClass]
    public class ProjectManagerTest {

        [TestMethod]
        public void ProjectManager_AddAndSaveOperation() {
            PluginManager pluginManager = new PluginManager();
            CoreSystem.AddManager(pluginManager);
            ProjectManager projectManager = new ProjectManager();
            projectManager.CreateDefaultProject();

            projectManager.Add(new Base.Plugins.Operation("DummyOperation"));
            using (MemoryStream stream = new MemoryStream()) {
                projectManager.Save(stream);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public void ProjectManager_Initialize() {
            ProjectManager projectManager = new ProjectManager();
        }

        [TestMethod]
        public void ProjectManager_SaveLoadDefaultProject() {
            ProjectManager projectManager = CoreSystem.FindManager<ProjectManager>();
            projectManager.CreateDefaultProject();

            using (MemoryStream stream = new MemoryStream()) {
                projectManager.Save(stream);
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());

                projectManager.Configuration = null;
                stream.Position = 0;
                projectManager.Configuration = projectManager.Load(stream);
                Assert.IsNotNull(projectManager.Configuration);
                Assert.AreEqual(projectManager.Configuration.Operations.Count, 1);
            }

            CoreSystem.Close();
        }
    }
}