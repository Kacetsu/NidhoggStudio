using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ns.Base;

namespace ns.Core.Manager {
    /// <summary>
    /// Manages the Devices added to the Application.
    /// </summary>
    public class DeviceManager : PluginManager {

        private List<Device> _configuratedDevices = new List<Device>();

        /// <summary>
        /// Initialize the DeviceManager.
        /// </summary>
        /// <returns></returns>
        public override bool Initialize() {
            return true;
        }

        /// <summary>
        /// Add Device.
        /// </summary>
        /// <param name="node">The Device to be added.</param>
        public override void Add(Base.Node node) {
            base.Add(node);
            this.OnSelectionChanged(node);
        }

        /// <summary>
        /// Remove Device.
        /// </summary>
        /// <param name="node">The Device to be removed.</param>
        public override void Remove(Node node) {
            base.Remove(node);
            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            if (projectManager.Configuration.Devices.Contains(node)) {
                projectManager.Configuration.Devices.Remove(node as Device);
            }
        }

        public override void OnSelectionChanged(object selectedObject) {
            Device device = selectedObject as Device;
            if (device == null)
                throw new NotSupportedException(selectedObject.GetType().ToString() + " is not supported at " + MethodInfo.GetCurrentMethod().ToString() + "!");

            ProjectManager projectManager = CoreSystem.Managers.Find(m => m.Name.Contains("ProjectManager")) as ProjectManager;
            if (!projectManager.Configuration.Devices.Contains(device)) {
                device.UID = Device.GenerateUID();
                projectManager.Configuration.Devices.Add(device);
            }
        }
    }
}
