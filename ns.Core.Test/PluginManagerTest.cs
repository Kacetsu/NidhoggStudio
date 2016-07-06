﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ns.Core.Manager;

namespace ns.Core.Test {

    [TestClass]
    public class PluginManagerTest {

        [TestMethod]
        public void PluginManager_Initialize() {
            Assert.IsTrue(CoreSystem.Instance.IsInitialized);
            PluginManager pluginManager = new PluginManager();
            CoreSystem.Close();
        }
    }
}