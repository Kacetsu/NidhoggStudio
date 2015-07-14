using ns.Base.Event;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ns.Core.Manager {
    public class DeviceManager : PluginManager {

        private List<Device> _configuratedDevices = new List<Device>();

        public override bool Initialize() {
            return true;
        }

        public override void Add(Base.Node node) {
            base.Add(node);
            this.OnSelectionChanged(node);
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
