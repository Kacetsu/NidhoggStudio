using ns.Base.Plugins.Devices;
using System;

namespace ns.Base.Event {

    public class DeviceSelectionChangedEventArgs : EventArgs {
        private Device _device;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSelectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DeviceSelectionChangedEventArgs(Device device) {
            _device = device;
        }

        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device {
            get { return _device; }
        }
    }
}