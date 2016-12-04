using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ns.Core.Test {

    [TestClass]
    public class CoreSystemTest {

        /// <summary>
        /// Cores the system_ initialize_ true.
        /// </summary>
        [TestMethod]
        public void CoreSystem_Initialize_True() {
            Assert.IsTrue(CoreSystem.Instance.TryInitialize());
            Assert.AreEqual(3, CoreSystem.Instance.Devices.Items.Count);
            Assert.AreEqual(6, CoreSystem.Instance.Plugins.Items.Count);
        }
    }
}