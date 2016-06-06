using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core.Test {

    [TestClass]
    public class PluginManagerTest {

        [TestMethod]
        public void PluginManager_Initialize() {
            Assert.IsTrue(CoreSystem.Initialize());
            PluginManager pluginManager = new PluginManager();
            Assert.IsTrue(pluginManager.Initialize());
        }
    }
}