using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ns.Base.Event {
    public class DeviceSelectionChangedEventArgs : EventArgs {
        private Device _device;

        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device {
            get { return _device; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceSelectionChangedEventArgs(Device device) {
            _device = device;
        }
    }
}
